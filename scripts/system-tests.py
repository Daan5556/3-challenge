"""Run from repository root after dotnet publish; uses an isolated database."""
import json, os, pathlib, socket, subprocess, tempfile, time, urllib.request, urllib.parse

publish = pathlib.Path(os.environ.get('PUBLISH_DIR', '/tmp/football-publish')).resolve()
with socket.socket() as sock:
    sock.bind(('127.0.0.1', 0))
    port = sock.getsockname()[1]
base = f'http://127.0.0.1:{port}'
with tempfile.TemporaryDirectory(prefix='football-system-') as tmp:
    env = dict(os.environ, Database__Path=f'{tmp}/club.db', ASPNETCORE_URLS=base)
    def start():
        proc = subprocess.Popen(['dotnet', str(publish / 'FootballClub.Server.dll')], cwd=publish, env=env, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        for _ in range(100):
            try:
                urllib.request.urlopen(base + '/api/status', timeout=1).close()
                return proc
            except OSError:
                if proc.poll() is not None: raise RuntimeError('Server exited at startup')
                time.sleep(.1)
        proc.terminate(); proc.wait()
        raise RuntimeError('Startup timeout')
    def get(path):
        with urllib.request.urlopen(base + path, timeout=5) as response: return response.read().decode()
    def state(): return json.loads(get('/api/status'))
    def post(path, data=None):
        return urllib.request.urlopen(base + path, urllib.parse.urlencode(data or {}).encode(), timeout=5).read().decode()
    def check(name, value):
        assert value, name
        print('PASS', name)
    proc = start()
    try:
        check('S01 dashboard and stylesheet', 'Football administration' in get('/') and len(get('/styles.css')) > 100)
        check('S02 seeded JSON status', state()['memberCount'] == 16)
        unpaid = next(c for c in state()['contributions'] if c['paidDate'] is None)
        post(f"/payments/{unpaid['id']}")
        check('S03 payment through HTTP', state()['overdueCount'] == 9)
        post('/memberships/1', {'teamId': ''})
        check('S04 membership through HTTP', next(m for m in state()['members'] if m['id'] == 1)['teamId'] is None)
        post('/memberships/1', {'teamId': '1'})
        post('/reminders')
        check('S05 repeated reminders', '0 reminder(s) generated.' in post('/reminders'))
        post('/matches/plan', {'teamId': '1', 'opponent': '<script>alert(1)</script>'})
        check('S06 planning and HTML escaping', len(state()['matches']) == 1 and '&lt;script&gt;' in get('/') and '<script>alert(1)</script>' not in get('/'))
        try: post('/matches/plan', {'teamId': 'invalid'})
        except urllib.error.HTTPError as error: check('S07 invalid planning input', error.code == 400)
        else: raise AssertionError('S07 expected HTTP 400')
        proc.terminate(); proc.wait(timeout=10)
        proc = start()
        check('S08 restart persistence', state()['overdueCount'] == 9 and len(state()['matches']) == 1)
    finally:
        proc.terminate(); proc.wait(timeout=10)

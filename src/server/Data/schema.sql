PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS teams (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE,
    minimum_players INTEGER NOT NULL CHECK (minimum_players > 0)
);
CREATE TABLE IF NOT EXISTS members (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    team_id INTEGER NULL REFERENCES teams(id) ON DELETE SET NULL
);
CREATE TABLE IF NOT EXISTS fields (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE
);
CREATE TABLE IF NOT EXISTS availability (
    member_id INTEGER NOT NULL REFERENCES members(id) ON DELETE CASCADE,
    available_date TEXT NOT NULL,
    available INTEGER NOT NULL CHECK (available IN (0, 1)),
    PRIMARY KEY (member_id, available_date)
);
CREATE TABLE IF NOT EXISTS contributions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    member_id INTEGER NOT NULL REFERENCES members(id) ON DELETE CASCADE,
    amount_cents INTEGER NOT NULL CHECK (amount_cents > 0),
    due_date TEXT NOT NULL,
    paid_date TEXT NULL
);
CREATE TABLE IF NOT EXISTS trainings (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    team_id INTEGER NOT NULL REFERENCES teams(id),
    field_id INTEGER NOT NULL REFERENCES fields(id),
    starts_at TEXT NOT NULL,
    ends_at TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS matches (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    team_id INTEGER NOT NULL REFERENCES teams(id),
    field_id INTEGER NOT NULL REFERENCES fields(id),
    starts_at TEXT NOT NULL,
    ends_at TEXT NOT NULL,
    opponent TEXT NOT NULL,
    status TEXT NOT NULL DEFAULT 'Planned'
);
CREATE TABLE IF NOT EXISTS reminders (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    contribution_id INTEGER NOT NULL REFERENCES contributions(id) ON DELETE CASCADE,
    sent_at TEXT NOT NULL,
    message TEXT NOT NULL,
    UNIQUE(contribution_id, sent_at)
);

# Soundwave Database Development Environment

This repository skeleton provides the database-side local development environment for Soundwave.

## What is included

- MySQL 8.4 in Docker
- Adminer for local database inspection
- Initial schema for core Soundwave entities
- Development seed data
- Makefile shortcuts
- GitHub Actions smoke test for schema bootstrapping
- Migration directory for future schema changes

## Prerequisites

- Docker Desktop or Docker Engine with Docker Compose
- Git
- Optional: MySQL CLI if you want to connect from the host machine

## Quick start

```bash
cp .env.example .env
docker compose up -d
```

Or:

```bash
make up
```

Check container status:

```bash
make status
```

Open Adminer at `http://localhost:8080` by default.

Use these development values unless you changed `.env`:

- System: `MySQL`
- Server: `db`
- Username: `soundwave`
- Password: `soundwave_dev_password`
- Database: `soundwave`

## Common commands

```bash
make up       # start the local database
make down     # stop containers
make logs     # follow MySQL logs
make shell    # open MySQL shell in the container
make seed     # load sample catalog data
make reset    # destroy local DB volume and rebuild from init scripts
make validate # validate docker-compose.yml
```

## Directory structure

```text
.
├── .env.example
├── .github/
│   └── workflows/
│       └── database-smoke-test.yml
├── database/
│   ├── init/
│   │   └── 001_schema.sql
│   ├── migrations/
│   │   └── README.md
│   ├── scripts/
│   │   └── wait-for-db.sh
│   └── seeds/
│       └── 001_dev_seed.sql
├── docker-compose.yml
├── Makefile
└── README.md
```

## Development workflow

1. Create a feature branch.
2. Make schema changes in a new migration file rather than modifying an already-shared migration.
3. Run `make reset` to test the database from a clean state.
4. Verify tables and constraints with `make shell`.
5. Commit through a pull request and request peer review.

## Security notes

- `.env` is ignored by Git and must not be committed.
- `.env.example` contains development-only credentials.
- Store password hashes, never plaintext passwords.
- Store refresh-token hashes rather than raw refresh tokens.
- Production secrets and TLS configuration belong in the deployment environment, not this repository.

## Next database tasks

Good follow-up tasks for the database implementation sprint:

- Add genres and track-genre relationships.
- Decide how album art and media file paths are stored.
- Finalize user settings and application settings tables.
- Add indexes based on real search/API queries.
- Add migration tooling such as Flyway once schema changes begin to accumulate.
- Add integration tests for constraints and common catalog/search queries.

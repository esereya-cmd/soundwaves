# Contributing

## Branching

Use short feature branches, for example:

- `db/dev-environment`
- `db/add-genres`
- `db/search-indexes`

## Pull requests

Every database change should include:

- A short explanation of the schema change
- The migration or SQL file
- Local verification steps
- Any rollback considerations
- Peer review before merge

## SQL conventions

- Use `snake_case` names.
- Use plural table names.
- Use `BIGINT UNSIGNED` surrogate primary keys for main entities.
- Use UTC timestamps.
- Add foreign keys for relational integrity.
- Add indexes for foreign keys and known search/filter paths.
- Do not store plaintext passwords or raw long-lived tokens.

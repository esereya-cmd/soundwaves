# Migrations

Place versioned SQL migration files here after the initial schema is established.

Suggested naming:

- `V001__initial_schema.sql`
- `V002__add_genres.sql`
- `V003__add_album_indexes.sql`

For the first sprint, `database/init/001_schema.sql` is enough to bootstrap a clean local database. Once multiple developers are changing the schema, move to a migration tool such as Flyway or Liquibase rather than editing an already-applied migration.

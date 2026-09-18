.PHONY: setup up down restart logs status reset seed shell validate

setup:
	@test -f .env || cp .env.example .env
	@echo "Created .env if it did not exist."

up: setup
	docker compose up -d

down:
	docker compose down

restart:
	docker compose restart

logs:
	docker compose logs -f db

status:
	docker compose ps

reset:
	docker compose down -v
	docker compose up -d

seed:
	docker compose exec -T db sh -c 'mysql -u"$$MYSQL_USER" -p"$$MYSQL_PASSWORD" "$$MYSQL_DATABASE"' < database/seeds/001_dev_seed.sql

shell:
	docker compose exec db sh -c 'mysql -u"$$MYSQL_USER" -p"$$MYSQL_PASSWORD" "$$MYSQL_DATABASE"'

validate:
	docker compose config -q

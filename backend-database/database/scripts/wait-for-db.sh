#!/usr/bin/env sh
set -eu

HOST="${MYSQL_HOST:-127.0.0.1}"
PORT="${MYSQL_PORT:-3306}"
USER="${MYSQL_USER:-soundwave}"
PASSWORD="${MYSQL_PASSWORD:-soundwave_dev_password}"

until mysqladmin ping -h "$HOST" -P "$PORT" -u "$USER" -p"$PASSWORD" --silent; do
  echo "Waiting for MySQL at $HOST:$PORT..."
  sleep 2
done

echo "MySQL is ready."

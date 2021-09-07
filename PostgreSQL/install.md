# PostgreSQL

## Installation

https://www.postgresql.org/download/

## Configuration

```bash
cd /var/lib/pgsql/13/data/

vim pg_hba.conf

host    all             all              0.0.0.0/0              md5

vim postgresql.conf

listen_addresses = '*'

```

## Shell

```bash
 sudo -i -u postgres

 psql

```

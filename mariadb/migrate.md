# Migrate

## From mysql 5.7

```bash

# 0. mariadb: backup

# 1. mariadb: stop master and slave
stop master;
stop slave;

# 2. mariadb: disable binlog
FLUSH LOGS;
PURGE LOGS;

# 3. mysql: mysqldump

mysqldump -u username -p  --databases dbname1 dbname2 > mysql-bak.sql

# or
mysqldump -h mysql_host -u root -p dbname1 | mysql -h mariadb_host -u root


CREATE USER 'mig'@'172.16.153.%' IDENTIFIED BY '123';
grant all ON *.*  TO 'mig'@'172.16.153.%'；
FLUSH PRIVILEGES;

# or
mysqldump  -h mysql_host -u mig -p'123' --databases dbname1 dbname2 | mysql -h mariadb_host -u root

# 4. create users

# 5. create master and slave


```
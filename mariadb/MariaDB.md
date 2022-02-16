# MariaDB

## Master and Slave

### Servers
* Master: 192.168.2.10
* Slave: 192.168.2.11

### Master

* /etc/mysql/mariadb.conf.d/50-server.cnf
```
[mysqld]
bind-address		= 0.0.0.0

server-id		= 1
log-bin
expire_logs_days	= 10
max_binlog_size   = 100M
log-basename      = master
binlog-format     = mixed
```
* user

```sql

CREATE USER 'rep'@'%' IDENTIFIED BY '123';
GRANT REPLICATION SLAVE ON *.* TO 'rep'@'%';
FLUSH PRIVILEGES;

FLUSH TABLES WITH READ LOCK;

SHOW MASTER STATUS; -- Get binlog file and position
select @@global.gtid_slave_pos, @@global.gtid_binlog_pos,@@global.gtid_current_pos;

UNLOCK TABLE;

```

### Slaves

* /etc/mysql/mariadb.conf.d/50-server.cnf
```
[mysqld]
bind-address		= 0.0.0.0

server-id		= 2
replicate_wild_do_table = yunqi.%
```

* sql
```sql
-- use MASTER_USE_GTID

-- Master 1
-- set gtid_slave_pos
SET GLOBAL gtid_slave_pos = "0-2-19554633";
CHANGE MASTER TO
  MASTER_HOST='172.16.153.178',
  MASTER_USER='rep',
  MASTER_PASSWORD='123',
  MASTER_PORT=3306,
  MASTER_USE_GTID=slave_pos,
  MASTER_CONNECT_RETRY=10;

-- Master2
CHANGE MASTER TO
  MASTER_HOST='172.16.153.177',
  MASTER_USER='rep',
  MASTER_PASSWORD='123',
  MASTER_PORT=3306,
  MASTER_USE_GTID=current_pos,
  MASTER_LOG_POS=760,
  MASTER_CONNECT_RETRY=10;

-- skip 1 error sql statement;
STOP SLAVE;
SET GLOBAL SQL_SLAVE_SKIP_COUNTER=1;
START SLAVE;
```

### skip replication

https://mariadb.com/kb/en/selectively-skipping-replication-of-binlog-events/
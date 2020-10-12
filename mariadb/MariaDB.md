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
-- get gtid_pos of Master
SELECT BINLOG_GTID_POS("master-bin.000876", 24635264);
-- set gtid_slave_pos
SET GLOBAL gtid_slave_pos = "0-1-2";
CHANGE MASTER TO
  MASTER_HOST='172.16.153.177',
  MASTER_USER='rep',
  MASTER_PASSWORD='123',
  MASTER_PORT=3306,
  MASTER_USE_GTID=current_pos
  MASTER_CONNECT_RETRY=10;

-- or use MASTER_LOG_FILE & MASTER_LOG_POS
CHANGE MASTER TO
  MASTER_HOST='172.16.153.177',
  MASTER_USER='rep',
  MASTER_PASSWORD='123',
  MASTER_PORT=3306,
  MASTER_LOG_FILE='master-bin.000003',
  MASTER_LOG_POS=760,
  MASTER_CONNECT_RETRY=10;

-- skip 1 error sql statement;
STOP SLAVE;
SET GLOBAL SQL_SLAVE_SKIP_COUNTER=1;
START SLAVE;
```

## MaxScale
https://mariadb.com/kb/en/mariadb-maxscale-20-readwrite-splitting-with-mysql-replication/

### Install

```bash
curl -sS https://downloads.mariadb.com/MariaDB/mariadb_repo_setup | sudo bash
dnf install maxscale # centos8
```

### Setup

* Create user for both Master and Slaves:
```sql
create user scalemonitor@'%' identified BY '123';
grant replication slave, replication client on *.* TO scalemonitor@'%';

create user maxscale@'%' identified BY '123';
grant select on mysql.*  TO maxscale@'%';
GRANT SHOW DATABASES ON *.* TO maxscale@'%';
```
* /etc/maxscale.cnf
```ini
# MaxScale documentation:
# https://mariadb.com/kb/en/mariadb-maxscale-24/

# Global parameters
#
# Complete list of configuration options:
# https://mariadb.com/kb/en/mariadb-maxscale-24-mariadb-maxscale-configuration-guide/

[maxscale]
threads=auto

# Server definitions
#
# Set the address of the server to the network
# address of a MariaDB server.
#

[server1]
type=server
address=192.168.2.10
port=3306
protocol=MariaDBBackend

[server2]
type=server
address=192.168.2.11
port=3306
protocol=MariaDBBackend


# Monitor for the servers
#
# This will keep MaxScale aware of the state of the servers.
# MariaDB Monitor documentation:
# https://mariadb.com/kb/en/mariadb-maxscale-24-mariadb-monitor/

[MariaDB-Monitor]
type=monitor
module=mariadbmon
servers=server1,server2
user=scalemonitor
password=123
monitor_interval=2000
detect_stale_master=true

# Service definitions
#
# Service Definition for a read-only service and
# a read/write splitting service.
#

# ReadConnRoute documentation:
# https://mariadb.com/kb/en/mariadb-maxscale-24-readconnroute/

#[Read-Only-Service]
#type=service
#router=readconnroute
#servers=server1
#user=myuser
#password=mypwd
#router_options=slave

# ReadWriteSplit documentation:
# https://mariadb.com/kb/en/mariadb-maxscale-24-readwritesplit/

[Read-Write-Service]
type=service
router=readwritesplit
servers=server1,server2
user=maxscale
password=123
max_slave_connections=100%

# Listener definitions for the services
#
# These listeners represent the ports the
# services will listen on.
#

#[Read-Only-Listener]
#type=listener
#service=Read-Only-Service
#protocol=MariaDBClient
#port=4008

[Read-Write-Listener]
type=listener
service=Read-Write-Service
protocol=MariaDBClient
port=4006


[MaxAdmin]
type=service
router=cli

[MaxAdmin-Unix-Listener]
type=listener
service=MaxAdmin
protocol=maxscaled
socket=default

```

### Monitor

``` bash
maxadmin

list servers

show service "Read-Write-Service"
```

## Log

### Slow query

## 性能

### 连接数

```sql
show variables like 'max_connections';
show global status like 'Connections';
show global status like 'Max_used_connections';
show global status like 'Thread_%';
```

# MaxScale
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
```
# MaxScale documentation:
# https://mariadb.com/kb/en/mariadb-maxscale-25/

# Global parameters
#
# Complete list of configuration options:
# https://mariadb.com/kb/en/mariadb-maxscale-25-mariadb-maxscale-configuration-guide/

[maxscale]
threads=auto

# MaxGui
# https://mariadb.com/kb/en/mariadb-maxscale-25-mariadb-maxscale-maxgui-guide/
#
# MaxGUI will be available on port 8989
# MaxGUI uses the same credentials as maxctrl, so by default, the username is admin and the password is mariadb.
admin_host=0.0.0.0
admin_secure_gui=false

# Server definitions
#
# Set the address of the server to the network
# address of a MariaDB server.
#

[server1]
type=server
address=172.16.153.177
port=3306
protocol=MariaDBBackend

[server2]
type=server
address=172.16.153.178
port=3306
protocol=MariaDBBackend

# Monitor for the servers
#
# This will keep MaxScale aware of the state of the servers.
# MariaDB Monitor documentation:
# https://mariadb.com/kb/en/maxscale-25-monitors/

[MariaDB-Monitor]
type=monitor
module=mariadbmon
servers=server1,server2
user=scalemonitor
password=123
monitor_interval=2000

# Service definitions
#
# Service Definition for a read-only service and
# a read/write splitting service.
#

# ReadConnRoute documentation:
# https://mariadb.com/kb/en/mariadb-maxscale-25-readconnroute/

#[Read-Only-Service]
#type=service
#router=readconnroute
#servers=server1
#user=myuser
#password=mypwd
#router_options=slave

# ReadWriteSplit documentation:
# https://mariadb.com/kb/en/mariadb-maxscale-25-readwritesplit/

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
```
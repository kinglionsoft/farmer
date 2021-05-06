# Monitoring

## Prometheus

* mysql user
```bash
CREATE USER 'exporter'@'127.0.0.1' IDENTIFIED BY '123456' WITH MAX_USER_CONNECTIONS 3;
GRANT PROCESS, REPLICATION CLIENT, SELECT ON *.* TO 'exporter'@'127.0.0.1';
FLUSH PRIVILEGES;
```

* my.cnf
```
[client]
user=exporter
password=123456
```

* mysqld-exporter.service

```
[Unit]
Description=mysql Monitoring System
Documentation=mysql Monitoring System

[Service]
ExecStart=/opt/prometheus/mysqld-exporter/mysqld_exporter-0.12.1.linux-amd64/mysqld_exporter \
    --config.my-cnf=/opt/prometheus/mysqld-exporter/my.cnf
    --collect.info_schema.processlist \
    --collect.info_schema.innodb_tablespaces \
    --collect.info_schema.innodb_metrics  \
    --collect.perf_schema.tableiowaits \
    --collect.perf_schema.indexiowaits \
    --collect.perf_schema.tablelocks \
    --collect.engine_innodb_status \
    --collect.perf_schema.file_events \
    --collect.binlog_size \
    --collect.info_schema.clientstats \
    --collect.perf_schema.eventswaits \
    --collect.slave_status \
    --collect.slave_hosts

[Install]
WantedBy=multi-user.target
```

* Prometheus config
```
- job_name: mysql
  static_configs:
  - targets: ['172.16.153.177:9104','172.16.153.178:9104']
```

* Grafana
Import dashboard: 11323

* Alerts

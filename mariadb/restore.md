# Backup & Restore

## mariabackup

### Manual
``` bash
# backup

mariabackup --user=root --backup --stream=xbstream | gzip > backupstream.gz

# restore

gunzip -c backupstream.gz | mbstream -x

mariabackup --prepare --target-dir=/var/mariadb/backup/

systemctl stop mariadb.service

# backup data directory
cp -rf /var/lib/mysql/xxx /data/backup/xxx

mariabackup --copy-back --target-dir=/var/mariadb/backup/

chown -R mysql:mysql /var/lib/mysql/

systemctl start mariadb.service
```

### Cron

``` bash
tee /data/backup/backup.sh <<EOF
KEEP=30

BACKUP_PATH="/data/backup"

TS=`date +"%Y-%m-%d_%H-%M-%S"`

BAK="${BACKUP_PATH}/${TS}.gz"

mariabackup --user=root --backup --stream=xbstream | gzip > $BAK

find ${BACKUP_PATH} -name "*.gz" -type f -mtime +$KEEP -delete
EOF

crontab -e
# backup mariadb at 23:00 every Staturday
0 23 * * 6 /data/backup/backup.sql

```

## xtrabackup

### 场景

* xtrabackup 全量备份的MySQL 5.7
* InnoDB
* 拥有完整的备份文件(.frm, .ibd)
* 拥有表结构

### 恢复

``` sql

-- 1. 创建数据库以及表, 略

-- 2. 删除表空间

ALTER TABLE table1 DISCARD TABLESPACE;

-- 3. 将备份的table1.ibd复制到数据库目录
-- 略

-- 4. s授权
sudo chmod 660 /var/lib/mysql/dbname/\*.ibd
sudo chown -R mysql:mysql /var/lib/mysql/dbname/

-- 5. 导入表空间
ALTER TABLE table1 IMPORT TABLESPACE;

```

###


## Errors

* InnoDB: Linux Native AIO disabled
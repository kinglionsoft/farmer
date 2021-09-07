# Redash 中文版

## 安装

### PostgreSQL

```bash

sudo -i -u postgres

psql

create database redash

create user redash with login password 'Ytzx22@@';

```

### k8s

* deploy

```bash
kubectl create -f - chinese.yaml
```

* create tables

```bash
kubectl exec <redash pod> -- /app/manage.py database create_tables

```

* create user

```bash
kubectl exec redash-tyreplus-8965f8b9d-9kvhl -- /app/manage.py users create --password '123456' 1138228664@qq.com '范天柱'
```

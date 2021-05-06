# The Bitnami Library of Redis Cluster for Kubernetes

https://github.com/bitnami/charts

## Master & Slave

### one master two slaves

* values.yaml

``` yaml
usePassword: false
volumePermissions:
  enabled: true
```

* PV

```yaml
apiVersion: v1
kind: PersistentVolume
metadata:
  name: redis-data-redis-master-0
  labels:
    app: redis
spec:
  capacity:
    storage: 8Gi
  accessModes:
    - ReadWriteOnce
  nfs:
    path: /nfsroot/redis/master
    server: 172.16.153.180
---
apiVersion: v1
kind: PersistentVolume
metadata:
  name: redis-data-redis-slave-0
  labels:
    app: redis
spec:
  capacity:
    storage: 8Gi
  accessModes:
    - ReadWriteOnce
  nfs:
    path: /nfsroot/redis/slave0
    server: 172.16.153.180
---
apiVersion: v1
kind: PersistentVolume
metadata:
  name: redis-data-redis-slave-1
  labels:
    app: redis
spec:
  capacity:
    storage: 8Gi
  accessModes:
    - ReadWriteOnce
  nfs:
    path: /nfsroot/redis/slave1
    server: 172.16.153.180
```

* install

```bash
helm repo add bitnami https://charts.bitnami.com/bitnami

helm install redis bitnami/redis -f values.yaml
```
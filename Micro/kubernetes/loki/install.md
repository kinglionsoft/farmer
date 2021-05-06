# Loki

# Installation
* https://grafana.com/docs/loki/latest/installation/helm/

``` bash
helm repo add grafana https://grafana.github.io/helm-charts
helm repo update

tee values.yaml << EOF

EOF

kubectl create ns loki

helm upgrade --install loki --namespace=loki grafana/loki -f values-loki.yaml

helm upgrade --install promtail --namespace=loki grafana/promtail -f values-promtail.yaml

kubectl create -f - <<EOF
apiVersion: v1
kind: PersistentVolume
metadata:
  name: pv-loki
  namespace: loki
  labels:
    app: loki
    app.kubernetes.io/name: loki
spec:
  storageClassName: ""
  claimRef:
    name: pvc-loki
    namespace: loki
  capacity:
    storage: 10Gi
  accessModes:
    - ReadWriteOnce
  nfs:
    path: /nfsroot/loki
    server: 172.16.153.180
---
kind: PersistentVolumeClaim
apiVersion: v1
metadata:
  name: pvc-loki
  namespace: loki
spec:
  storageClassName: ""
  volumeName: pv-loki
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 10Gi
EOF

#172.16.153.180

persistentVolumeClaim:
          claimName: pvc-file


```
* nfs

``` bash
sudo mkdir /nfsroot/loki
# https://github.com/grafana/loki/blob/master/cmd/loki/Dockerfile#L18
sudo chown -R 10001:10001 /nfsroot/loki
```

* Grafana

> https://grafana.com/docs/loki/latest/getting-started/grafana/

https://grafana.com/grafana/dashboards/12611

```

```

## multiline
Change stages from **docker** to **multiline** on kubernetes-pods-direct-controllers

``` bash
kubectl edit cm loki-promtail -n loki
```

``` yaml
    - job_name: kubernetes-pods-direct-controllers
      pipeline_stages:
       # - docker: {}
        - multiline:
            firstline: '^time="\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}"'
            max_wait_time: 2s
```
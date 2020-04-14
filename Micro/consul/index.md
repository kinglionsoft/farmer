# Running Consul on Kubernetes

[https://github.com/kelseyhightower/consul-on-kubernetes](https://github.com/kelseyhightower/consul-on-kubernetes)

### NodeSelector for StatefulSet

```
# affinity
nodeAffinity:
  requiredDuringSchedulingIgnoredDuringExecution:
    nodeSelectorTerms:
    - matchExpressions:
        - key: beta.kubernetes.io/os
          operator: In
          values:
            - linux


```

``` bash

curl -s -L -o /bin/cfssl https://pkg.cfssl.org/R1.2/cfssl_linux-amd64 \
curl -s -L -o /bin/cfssljson https://pkg.cfssl.org/R1.2/cfssljson_linux-amd64 \
curl -s -L -o /bin/cfssl-certinfo https://pkg.cfssl.org/R1.2/cfssl-certinfo_linux-amd64 \
chmod +x /bin/cfssl*



tee data-consul-0-pv.yaml << EOF
kind: PersistentVolume
apiVersion: v1
metadata:
  name: data-consul-0
  labels:
    type: local
spec:
  storageClassName: manual
  capacity:
    storage: 10Gi
  accessModes:
    - ReadWriteOnce
  hostPath:
    path: "/data/consul/data0"

---    
kind: PersistentVolumeClaim
apiVersion: v1
metadata:
  name: data-consul-0
spec:
  storageClassName: manual
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 10Gi
EOF

kubectl create -f data-consul-0-pv.yaml 


kubectl port-forward consul-0 --address=0.0.0.0 8500:8500
```

### Expose


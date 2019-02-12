# Running Consul on Kubernetes

[https://github.com/kelseyhightower/consul-on-kubernetes](https://github.com/kelseyhightower/consul-on-kubernetes)

``` bash

curl -s -L -o /bin/cfssl https://pkg.cfssl.org/R1.2/cfssl_linux-amd64 \
curl -s -L -o /bin/cfssljson https://pkg.cfssl.org/R1.2/cfssljson_linux-amd64 \
curl -s -L -o /bin/cfssl-certinfo https://pkg.cfssl.org/R1.2/cfssl-certinfo_linux-amd64 \
chmod +x /bin/cfssl*



tee data-consul-0-pv.yaml << EOF
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

```


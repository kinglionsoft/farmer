# TLS

## Istio
阿里云的istio版本为1.2.7，不支持SDS，需要使用File Mount的方式加载证书。

``` bash

# 创建证书

kubectl create -n istio-system secret tls istio-ingressgateway-test666-certs --key test666.com.key --cert test666.com.pem
kubectl create -n istio-system secret tls istio-ingressgateway-testfile-certs --key testfile.com.key --cert testfile.com.pem

# Mount
cat > gateway-patch.json <<EOF
[
{
  "op": "add",
  "path": "/spec/template/spec/containers/0/volumeMounts/0",
  "value": {
    "mountPath": "/etc/istio/ingressgateway-test666-certs",
    "name": "ingressgateway-test666-certs",
    "readOnly": true
  }
},
{
  "op": "add",
  "path": "/spec/template/spec/volumes/0",
  "value": {
  "name": "ingressgateway-test666-certs",
    "secret": {
      "secretName": "istio-ingressgateway-test666-certs",
      "optional": true
    }
  }
},
{
  "op": "add",
  "path": "/spec/template/spec/containers/0/volumeMounts/0",
  "value": {
    "mountPath": "/etc/istio/ingressgateway-testfile-certs",
    "name": "ingressgateway-testfile-certs",
    "readOnly": true
  }
},
{
  "op": "add",
  "path": "/spec/template/spec/volumes/0",
  "value": {
  "name": "ingressgateway-testfile-certs",
    "secret": {
      "secretName": "istio-ingressgateway-testfile-certs",
      "optional": true
    }
  }
}
]
EOF

kubectl -n istio-system patch --type=json deploy istio-ingressgateway -p "$(cat gateway-patch.json)"
```

## k8s ingress

### Ingress Nginx全局配置

https://kubernetes.github.io/ingress-nginx/user-guide/nginx-configuration/configmap/#use-forwarded-headers
``` bash
kubectl edit configmap nginx-configuration -n kube-system

```

### 代理配置
```bash
kubectl create secret tls test666-certs --key test666.com.key --cert test666.com.pem
kubectl create secret tls testfile-certs --key testfile.com.key --cert testfile.com.pem

kubectl create -f - <<EOF
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: core-web
  annotations:
    nginx.ingress.kubernetes.io/from-to-www-redirect: "true"
spec:
  tls:
    - secretName: test666-certs
  rules:
  - host: www.test666.com
    http:
      paths:
      - backend:
          serviceName: core-web
          servicePort: 80
  - host: openapi.test666.com
    http:
      paths:
      - backend:
          serviceName: core-webapi
          servicePort: 80
  - host: auth.test666.com
    http:
      paths:
      - backend:
          serviceName: core-auth
          servicePort: 80
EOF

kubectl create -f - <<EOF
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: test-web
  annotations:
    nginx.ingress.kubernetes.io/from-to-www-redirect: "true"
spec:
  tls:
    - secretName: testfile-certs
  rules:
  - host: www.testfile.com
    http:
      paths:
      - backend:
          serviceName: test-web
          servicePort: 80
  - host: openapi.testfile.com
    http:
      paths:
      - backend:
          serviceName: core-webapi
          servicePort: 80
  - host: auth.testfile.com
    http:
      paths:
      - backend:
          serviceName: core-auth
          servicePort: 80
  - host: api.testfile.com
    http:
      paths:
      - backend:
          serviceName: core-web
          servicePort: 80
  - host: world-api.testfile.com
    http:
      paths:
      - backend:
          serviceName: core-web
          servicePort: 80
  - host: world.testfile.com
    http:
      paths:
      - backend:
          serviceName: test-web
          servicePort: 80
EOF

```

### Test headers

``` bash
kubectl create -f - <<EOF
apiVersion: extensions/v1beta1
kind: Deployment
metadata:
  labels:
    app: web3
  name: web3
spec:
  replicas: 1
  template:
    metadata:
      labels:
        app: web3
      name: web3
    spec:
      containers:
      - name: web3
        imagePullPolicy: IfNotPresent
        image: registry.cn-shenzhen.aliyuncs.com/test/web3
        ports:
        - containerPort: 80    
---
apiVersion: v1
kind: Service
metadata:
  name: web3
  labels:
    app: web3
spec:
  ports:
  - port: 80
    targetPort: 80
    name: http
  selector:
    app: web3
---
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: web3
spec:
  tls:
    - secretName: testfile-certs
  rules:
  - host: web3.testfile.com
    http:
      paths:
      - backend:
          serviceName: web3
          servicePort: 80

EOF
```
# TLS

## Istio
阿里云的istio版本为1.2.7，不支持SDS，需要使用File Mount的方式加载证书。

``` bash

# 创建证书

kubectl create -n istio-system secret tls istio-ingressgateway-ctc666-certs --key ctc666.com.key --cert ctc666.com.pem
kubectl create -n istio-system secret tls istio-ingressgateway-ctcfile-certs --key ctcfile.com.key --cert ctcfile.com.pem

# Mount
cat > gateway-patch.json <<EOF
[
{
  "op": "add",
  "path": "/spec/template/spec/containers/0/volumeMounts/0",
  "value": {
    "mountPath": "/etc/istio/ingressgateway-ctc666-certs",
    "name": "ingressgateway-ctc666-certs",
    "readOnly": true
  }
},
{
  "op": "add",
  "path": "/spec/template/spec/volumes/0",
  "value": {
  "name": "ingressgateway-ctc666-certs",
    "secret": {
      "secretName": "istio-ingressgateway-ctc666-certs",
      "optional": true
    }
  }
},
{
  "op": "add",
  "path": "/spec/template/spec/containers/0/volumeMounts/0",
  "value": {
    "mountPath": "/etc/istio/ingressgateway-ctcfile-certs",
    "name": "ingressgateway-ctcfile-certs",
    "readOnly": true
  }
},
{
  "op": "add",
  "path": "/spec/template/spec/volumes/0",
  "value": {
  "name": "ingressgateway-ctcfile-certs",
    "secret": {
      "secretName": "istio-ingressgateway-ctcfile-certs",
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
kubectl create secret tls ctc666-certs --key ctc666.com.key --cert ctc666.com.pem
kubectl create secret tls ctcfile-certs --key ctcfile.com.key --cert ctcfile.com.pem

kubectl create -f - <<EOF
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: core-web
  annotations:
    nginx.ingress.kubernetes.io/from-to-www-redirect: "true"
spec:
  tls:
    - secretName: ctc666-certs
  rules:
  - host: www.ctc666.com
    http:
      paths:
      - backend:
          serviceName: core-web
          servicePort: 80
  - host: openapi.ctc666.com
    http:
      paths:
      - backend:
          serviceName: core-webapi
          servicePort: 80
  - host: auth.ctc666.com
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
  name: ctc-web
  annotations:
    nginx.ingress.kubernetes.io/from-to-www-redirect: "true"
spec:
  tls:
    - secretName: ctcfile-certs
  rules:
  - host: www.ctcfile.com
    http:
      paths:
      - backend:
          serviceName: ctc-web
          servicePort: 80
  - host: openapi.ctcfile.com
    http:
      paths:
      - backend:
          serviceName: core-webapi
          servicePort: 80
  - host: auth.ctcfile.com
    http:
      paths:
      - backend:
          serviceName: core-auth
          servicePort: 80
  - host: api.ctcfile.com
    http:
      paths:
      - backend:
          serviceName: core-web
          servicePort: 80
  - host: world-api.ctcfile.com
    http:
      paths:
      - backend:
          serviceName: core-web
          servicePort: 80
  - host: world.ctcfile.com
    http:
      paths:
      - backend:
          serviceName: ctc-web
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
        image: registry.cn-shenzhen.aliyuncs.com/ctc/web3
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
    - secretName: ctcfile-certs
  rules:
  - host: web3.ctcfile.com
    http:
      paths:
      - backend:
          serviceName: web3
          servicePort: 80

EOF
```
# Traefix
traefik 是一个前端负载均衡器，对于微服务架构尤其是 kubernetes 等编排工具具有良好的支持；同 nginx 等相比，traefik 能够自动感知后端容器变化，从而实现自动服务发现。

## 创建证书


## 部署

``` bash
# 配置ClusterRole以及ClusterRoleBinding来对api-server的进行相应权限的鉴权
kubectl create -f https://raw.githubusercontent.com/containous/traefik/master/examples/k8s/traefik-rbac.yaml
# 部署
tee traefik-deployment.yaml << EOF
---
apiVersion: v1
kind: ServiceAccount
metadata:
  name: traefik-ingress-controller
  namespace: kube-system
---
kind: Deployment
apiVersion: extensions/v1beta1
metadata:
  name: traefik-ingress-controller
  namespace: kube-system
  labels:
    k8s-app: traefik-ingress-lb
spec:
  replicas: 1
  selector:
    matchLabels:
      k8s-app: traefik-ingress-lb
  template:
    metadata:
      labels:
        k8s-app: traefik-ingress-lb
        name: traefik-ingress-lb
    spec:
      hostNetwork: true
      serviceAccountName: traefik-ingress-controller
      terminationGracePeriodSeconds: 60
      containers:
      - image: traefik
        name: traefik-ingress-lb
        ports:
        - name: http
          containerPort: 80
          hostPort: 80
        - name: admin
          containerPort: 8080
          hostPort: 8080
        args:
        - --web
        - --kubernetes
        - --logLevel=INFO
      nodeSelector:
        kubernetes.io/hostname: eson-entry
EOF
kubectl create -f traefik-deployment.yaml 
# create UI
tee traefik-ui.yaml << EOF
apiVersion: v1
kind: Service
metadata:
  name: traefik-web-ui
  namespace: kube-system
spec:
  selector:
    k8s-app: traefik-ingress-lb
  ports:
  - name: web
    port: 80
    targetPort: 8080
EOF
kubectl create -f traefik-ui.yaml
```

## 代理

``` bash

tee file-store-traefik.yaml << EOF
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: file-store-traefik
  namespace: default
  annotations:
    kubernetes.io/ingress.class: traefik
spec:
  rules:
  - host: store.yitu666.com
    http:
      paths:
      - path: /
        backend:
          serviceName: file-store
          servicePort: 8082
EOF
kubectl create -f file-store-traefik.yaml
```

## 清除

``` bash
kubectl delete -f file-store-traefik.yaml
kubectl delete -f traefik-ui.yaml
kubectl delete -f traefik-deployment.yaml 
kubectl delete -f https://raw.githubusercontent.com/containous/traefik/master/examples/k8s/traefik-rbac.yaml
```
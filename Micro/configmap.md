# ConfigMap

# Create
```bash
kubectl create configmap file-store-config --from-file=appsettings.k8s.json

kubectl get configmap file-store-config -o yaml
```

# Upload

```bash
kubectl edit configmap file-store-config
```
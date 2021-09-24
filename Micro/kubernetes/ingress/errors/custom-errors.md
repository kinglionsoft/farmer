# Custom error pages

https://kubernetes.github.io/ingress-nginx/examples/customization/custom-errors/

## Default backend

```yaml
---

---
kind: PersistentVolumeClaim
apiVersion: v1
metadata:
  name: redis-default-backend
  namespace: kube-system
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 1Gi
---
apiVersion: v1
kind: PersistentVolume
metadata:
  name: redis-default-backend
spec:
  capacity:
    storage: 1Gi
  accessModes:
    - ReadWriteOnce
  nfs:
    path: /nfsroot/nginx
    server: 172.16.153.180
---

```
# Errors

### BIRD is not ready: BGP not established

#### Troubleshoot

```bash
kubectl describe pod calico-node-f7zkc -n kube-system
```

```
 Warning  Unhealthy       30m (x4 over 30m)      kubelet, k8s-master  (combined from similar events): Readiness probe failed: calico/node is not ready: BIRD is not ready: BGP not established with 192.168.2.25,192.168.2.262021-06-23 03:14:08.390 [INFO][535] health.go 156: Number of node(s) with BGP peering established = 0

```

#### Reason

此节点上配置了多网卡，导致calio没有找到正确的网卡

#### Resolution
通过环境变量指定网卡

```yaml
- name: IP_AUTODETECTION_METHOD
  value: "interface=ens160"
``




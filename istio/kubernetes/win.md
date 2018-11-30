# Using Windows Server Containers in Kubernetes

* 参考: https://docs.microsoft.com/en-us/virtualization/windowscontainers/kubernetes/getting-started-kubernetes-windows

## 安装

* Windows Server 2019, 192.168.0.248, Administrator/Yx123456
* 确保升级到：10.0.17763.134. http://catalog.update.microsoft.com/
* docker 要安装docker ee !!
* Join node

``` bash
cd c:\k
chcp 65001
.\start.ps1 -ManagementIP 192.168.0.248 -ClusterCIDR 10.244.0.0/16 -ServiceCIDR 10.96.0.0/12 -KubeDnsServiceIP 10.96.0.10
```
## 配置为服务
* https://kubernetes.io/docs/getting-started-guides/windows/#kubelet-and-kube-proxy-can-now-run-as-windows-services

* 将配置写入配置文件：部分配置需要从配置文件读取

``` js
// c:\k\kubelet-config.yaml
kind: KubeletConfiguration
apiVersion: kubelet.config.k8s.io/v1beta1
cgroupsPerQOS: false
resolverConfig: ""
enforceNodeAllocatable: []
enableDebuggingHandlers: true
clusterDNS: 
  - 10.96.0.10
clusterDomain: cluster.local
hairpinMode: promiscuous-bridge

```
* 设置环境变量：KUBE_NETWORK=cbr0 NODE_NAME=kube-win-1

* 启动flanneld. 这个不支持用sc.exe创建服务，使用【任务计划程序】在开机时启动以下脚本。**有时要重启多次才能成功，如何解决**

```
Param(
    [parameter(Mandatory = $false)] $ManagementIP = "192.168.0.248"
)
chcp 65001
$NetworkMode = "L2Bridge"
$NetworkName = "cbr0"
ipmo C:\k\hns.psm1
ipmo C:\k\helper.psm1

# Create a L2Bridge to trigger a vSwitch creation. Do this only once
if(!(Get-HnsNetwork | ? Name -EQ "External"))
{
    New-HNSNetwork -Type $NetworkMode -AddressPrefix "192.168.255.0/30" -Gateway "192.168.255.1" -Name "External" -Verbose
}

StartFlanneld -ipaddress $ManagementIP -NetworkName $NetworkName
```

* 创建服务
``` bash
# kubelet
sc.exe create kubelet binPath= "c:\k\kubelet.exe --windows-service --hostname-override=$(hostname) --v=4 --pod-infra-container-image=kubeletwin/pause --allow-privileged=true --kubeconfig=c:\k\config  --image-pull-progress-deadline=20m               --network-plugin=cni --cni-bin-dir=""c:\k\cni"" --cni-conf-dir ""c:\k\cni\config"" --resolv-conf="""" --config=""c:\k\kubelet-config.yaml"" " start= auto 
# kube-proxy
sc.exe create kube-proxy binPath="c:\k\kube-proxy.exe --windows-service --proxy-mode=kernelspace --hostname-override=$(hostname) --kubeconfig=c:\k\config" depend= "kubelet" start= auto 
```

## Errors

#### ContainerCreating：The container operating system does not match the host operating system.
* Windows 使用 microsoft/nanoserver:latest 来构建 kubeletwin/pause:latest。要确保nanoserver的版本与主机的操作系统版本一致。

``` bash
cd c:\k
docker rmi -f kubeletwin/pause:latest
docker pull mcr.microsoft.com/windows/nanoserver:1809
docker tag mcr.microsoft.com/windows/nanoserver:1809 microsoft/nanoserver:latest
docker build -t kubeletwin/pause .
```

#### 无法访问pod ip
* 这个windows节点是VMware启动的虚拟机，需要开启虚拟网卡的 混杂模式。https://kb.vmware.com/s/article/1004099
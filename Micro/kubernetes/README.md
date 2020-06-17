# Kubernetes 约定

对Docker、k8s的编排和部署进行约定。

[[_TOC_]]

## Docker私有仓储

### 访问
* 使用[Harbor](https://github.com/goharbor/harbor)搭建，访问地址为：[http://registry.local.com](http://registry.local.com)
* 未使用https，故需要将此仓储加入到 insecure-registries，参考：

``` json
# Ubuntu 18.04
# /etc/docker/daemon.json
{
    "insecure-registries" : ["registry.local.com"],
    "registry-mirrors": ["https://registry.docker-cn.com"]
}

```
### 认证

认证模式为：LDAP, 使用域账户进行登录

``` bash
docker login registry.local.com
```

### 生产环境镜像
混合云环境下，Docker私有仓储部署在私有云中，但生产环境在公有云（阿里云），有2种方案实现部署：

* 方案一：阿里云DNS私有域中，将registry.local.com指向私有云中仓储服务器。**缺点**：k8s节点都需要从私有云仓储拉取镜像，受混合云带宽限制，首次拉取时速度较慢。
* **【推荐】**方案二：在阿里云中部署镜像仓储，私有云中仓储服务器可将生产环境镜像同步到阿里云仓储，将registry.local.com指向阿里云云中仓储服务器，k8s节点直接从阿里云的镜像仓储中拉取镜像。**缺点**：生产环境有不同的镜像源，需要将私有云中的镜像同步到阿里云。

## 集群基础设施

集群基础设施服务的部署文件统一存储在k8s/目录下，包括RabbitMQ、Redis、Jaeger、FTP、Ocelot\HttpGateway等。

### 集群环境变量
当服务在集群中启动时，可以读取集群提供的环境变量
* 

### 统一配置
集群中的服务统一使用consul + 环境变量进行配置，不再读取本地配置文件（如：appsettings.json）。


## TFS管道

### Docker镜像生成代理
Docker生成代理部署在Ubuntu18.04上，与Docker私有仓储(registry.local.com)位于同一台虚拟机。

#### 依赖
* vsts agent
* Docker
* DotNetCore SDK
* [kubectl](https://k8smeetup.github.io/docs/tasks/tools/install-kubectl/) 用于执行k8s上的服务部署作业

### 基本镜像

#### 官方镜像
代理机上已经下载部分常用的镜像：
* mcr.microsoft.com<span></span>/dotnet/core/aspnet:3.0
* mcr.microsoft.com<span></span>/dotnet/core/aspnet:2.2
* mcr.microsoft.com<span></span>/dotnet/core/aspnet:2.1
* mcr.microsoft.com<span></span>/dotnet/core/runtime:3.0
* mcr.microsoft.com<span></span>/dotnet/core/runtime:2.2
* mcr.microsoft.com<span></span>/dotnet/core/runtime:2.1

#### GDI+ 镜像
涉及到图像操作时，需要在Docker镜像中安装GDI+的依赖包；涉及到字体渲染时，需要将字体文件复制到Docker镜像中。详见生成脚本：[build-docker-base-image.sh](./build-docker-base-image.sh)
* registry.local.com<span></span>/dotnetcore/aspnet-gdi:3.0
* registry.local.com<span></span>/dotnetcore/aspnet-gdi:2.2
* registry.local.com<span></span>/dotnetcore/aspnet-gdi:2.1
* registry.local.com<span></span>/dotnetcore/runtime-gdi:3.0
* registry.local.com<span></span>/dotnetcore/runtime-gdi:2.2
* registry.local.com<span></span>/dotnetcore/runtime-gdi:2.1
* registry.local.com<span></span>/dotnetcore/aspnet-ttf:3.0
* registry.local.com<span></span>/dotnetcore/aspnet-ttf:2.2
* registry.local.com<span></span>/dotnetcore/aspnet-ttf:2.1
* registry.local.com<span></span>/dotnetcore/runtime-ttf:3.0
* registry.local.com<span></span>/dotnetcore/runtime-ttf:2.2
* registry.local.com<span></span>/dotnetcore/runtime-ttf:2.1

> ttf镜像包含100+MB的字体文件

### Docker镜像生成管道

#### 生成和发布
生成和发布的管道参考

* 完成Docker镜像打包上传后，需要删除输出目录，加快发布速度；
* 只需要将k8s的部署配置文件发布。

#### Docker镜像标签自动化

* 使用.csproj中的Version作为Docker镜像的标签，便于跟踪；**发布Docker镜像前必须更新项目版本号**
* 生成的镜像名称需要同步更新到k8s的部署配置文件中（.yaml)
* TFS Pipeline 自定义变量名：[TFS custom-variables](https://docs.microsoft.com/en-us/azure/devops/pipelines/release/variables?view=azure-devops&tabs=shell#custom-variables)
* 镜像的标签后缀名为：**BuildID**，例如：registry.local.com/test/ocelot:2.0.0.1773

``` bash

aws eks --region ap-northeast-2 update-kubeconfig --name ks

# install eksctl

# create ssh key: sg


eksctl utils associate-iam-oidc-provider \
    --region ap-northeast-2 \
    --cluster ks \
    --approve


eksctl create iamserviceaccount \
    --region ap-northeast-2 \
    --name alb-ingress-controller \
    --namespace kube-system \
    --cluster ks \
    --attach-policy-arn arn:aws:iam::527118913526:policy/ALBIngressControllerIAMPolicy \
    --override-existing-serviceaccounts \
    --approve    
```

``` yaml
apiVersion: eksctl.io/v1alpha5
kind: ClusterConfig
metadata:
  name: ks
  region: ap-northeast-2
vpc:
  id: "vpc-93e30ef8"
  cidr: "172.31.0.0/16"
  subnets:
    private:
      ap-northeast-2c:
          id: "subnet-aa9bd8e6"
          cidr: "172.31.16.0/20"
      ap-northeast-2b:
          id: "subnet-c0bf41bb"
          cidr: "172.31.32.0/20"
      ap-northeast-2a:
          id: "subnet-db203fb3"
          cidr: "172.31.0.0/20"

nodeGroups:
  - name: ks-workers
    instanceType: t3.large
    desiredCapacity: 2
    privateNetworking: false
    securityGroups:
      withShared: true
      withLocal: true
      attachIDs: ['sg-05d6a5ecd9358e263', 'sg-09057153a351f365b']
    ssh:
      publicKeyName: 'ctc_korea'
```
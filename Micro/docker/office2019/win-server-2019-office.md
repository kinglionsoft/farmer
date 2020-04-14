# 带Office的Windows Server 2019镜像


## 生成

* 下载 Office Deployment Tool: [https://www.microsoft.com/en-us/download/details.aspx?id=49117](https://www.microsoft.com/en-us/download/details.aspx?id=49117)

* 解压得到：setup.exe、configuration-Office2019Enterprise.xml

* 修改configuration-Office2019Enterprise.xml内容， 参考 https://config.office.com/

``` xml
<Configuration>
  <Add OfficeClientEdition="64" Channel="PerpetualVL2019">
    <Product ID="ProPlus2019Volume">
      <Language ID="en-us" />
      <ExcludeApp ID="Visio" />
      <ExcludeApp ID="Skype" />
      <ExcludeApp ID="Skypeforbusiness" />
      <ExcludeApp ID="Access" />
      <ExcludeApp ID="Groove" />
      <ExcludeApp ID="Lync" />
      <ExcludeApp ID="OneDrive" />
      <ExcludeApp ID="OneNote" />
      <ExcludeApp ID="Outlook" />
      <ExcludeApp ID="Teams" />
      <ExcludeApp ID="Publisher" />
    </Product>
  </Add>
  <RemoveMSI All="True" /> 
  <Display Level="None" AcceptEULA="TRUE" />
  <!--  <Property Name="AUTOACTIVATE" Value="1" />  -->
</Configuration>
```

* Dockerfile
https://forums.docker.com/t/appcrash-kernelbase-dll-error-when-i-try-to-use-microsoft-office-in-docker-container/25706/2
```


```
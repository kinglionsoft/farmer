FROM mcr.microsoft.com/windows/servercore:1809

MAINTAINER yangchao yangch_ts@qq.com

# Install Office

COPY setup.exe office2019.xml  C:/office/

RUN C:/office/setup.exe /configure office2019.xml

# Install Printer

SHELL ["powershell"]

RUN Install-WindowsFeature Print-Server ; \
    Set-Service spooler -StartupType Automatic ; \
    Start-Service spooler

# Copy missed DLLs

ENTRYPOINT powershell.exe
docker run -it --platform=linux -v c:\Users\jamessu\AppData\Roaming\Microsoft\UserSecrets\KubeCalc-UserSecrets\:/etc/secrets --rm -e ClusterId=mycluster -p 40000:40000 kubecalcsilo:latest

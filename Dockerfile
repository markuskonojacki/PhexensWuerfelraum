FROM ghcr.io/linuxserver/baseimage-mono:focal

RUN    echo "**** install packages ****" \
    && apt-get update \
    && apt-get install -y \
       unzip \
    && echo "**** install server ****" \
    && mkdir -p /app/phexenswuerfelraum \
    && cd /app/phexenswuerfelraum \
    && SERVER_VERSION=$(curl -s https://api.github.com/repos/markuskonojacki/PhexensWuerfelraum/releases/latest | grep -oP '"tag_name": "\K(.*)(?=")') \
    && curl -o /tmp/phexenswuerfelraum.zip -L "https://github.com/markuskonojacki/PhexensWuerfelraum/releases/download/${SERVER_VERSION}/Server-Linux-${SERVER_VERSION}.zip" \
    && unzip /tmp/phexenswuerfelraum.zip -d /app/phexenswuerfelraum \
    && echo "**** cleanup ****" \
    && apt-get clean \
    && rm -rf \
       /tmp/* \
       /var/tmp/*

# add local files
COPY root/ /

# ports and volumes
EXPOSE 12113
VOLUME /config
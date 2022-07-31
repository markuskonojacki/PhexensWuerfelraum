FROM mono:6.12

ARG PUID=1000
ARG PGID=1000

RUN    apt-get update && apt-get install --no-install-recommends --yes \
       wget \
       unzip \
	  && mkdir -p /app/config \
    && addgroup \
       --gid "$PGID" \
       phex \
    && adduser \
       --disabled-password \
       --gecos "" \
       --home /app \
       --ingroup phex \
       --uid "$PUID" \
       phex \
    && cd /app \
    && wget -q https://github.com/markuskonojacki/PhexensWuerfelraum/releases/download/$(curl -s https://api.github.com/repos/markuskonojacki/PhexensWuerfelraum/releases/latest | grep -oP '"tag_name": "\K(.*)(?=")')/Server-Linux-$(curl -s https://api.github.com/repos/markuskonojacki/PhexensWuerfelraum/releases/latest | grep -oP '"tag_name": "\K(.*)(?=")').zip \
    && unzip -oq Server-Linux-*.zip \
    && rm -rf Server-Linux-*.zip \
    && chmod +x PhexensWuerfelraum.Server.Console

COPY settings.example.ini /app/config/

RUN chown -R phex:phex /app

WORKDIR /app

USER phex

EXPOSE 12113

CMD [ "/app/PhexensWuerfelraum.Server.Console" ]
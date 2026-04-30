FROM node:alpine
RUN npm install -g mountebank --omit=dev
WORKDIR /mb
COPY imposter.json /mb/imposter.json
EXPOSE 2525 3000-3010
ENTRYPOINT ["mb"]
CMD ["--configfile", "/mb/imposter.json"]
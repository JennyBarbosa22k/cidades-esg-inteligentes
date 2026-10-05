# ---------- Estagio 1: build ----------
FROM maven:3.9-eclipse-temurin-17 AS build
WORKDIR /app
COPY pom.xml .
RUN mvn -B -q dependency:go-offline
COPY src ./src
RUN mvn -B -q clean package -DskipTests

# ---------- Estagio 2: runtime (imagem enxuta) ----------
FROM eclipse-temurin:17-jre-alpine
WORKDIR /app
RUN addgroup -S esg && adduser -S esg -G esg
COPY --from=build /app/target/cidades-esg-*.jar app.jar
USER esg
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
  CMD wget -qO- http://localhost:8080/actuator/health || exit 1
ENTRYPOINT ["java", "-jar", "app.jar"]

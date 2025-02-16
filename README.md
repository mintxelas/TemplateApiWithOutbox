# SampleApiWithOutbox

***Nota:* README generado por ChatGPT (y mucha ayuda humana)**

Este repositorio implementa una API desarrollada con ASP.NET Core que utiliza los patrones Outbox y Backend for Frontend (BFF) para garantizar consistencia en la comunicación entre servicios y mejorar la seguridad en aplicaciones web.

## Arquitectura del Proyecto

El proyecto está organizado en los siguientes directorios:

- **src/**: Contiene el código fuente de la aplicación.
  - **Api/**: Proyecto principal de la API del backend desarrollada con ASP.NET Core. Se ha diseñado siguiendo los principios de "clean architecture".
  - **Front/**: Implementa el patrón Backend for Frontend (BFF) para gestionar la comunicación entre el front-end y la API del backend.
- **tests/**: Contiene los proyectos de pruebas unitarias y de integración del backend.

## Patrón Outbox

En arquitecturas de microservicios, la comunicación asíncrona es esencial para mantener la consistencia entre los servicios. El patrón Outbox garantiza que las operaciones de base de datos y la publicación de mensajes se realicen de manera atómica, evitando inconsistencias.

### Flujo de Trabajo:

1. **Recepción de Solicitud**: Un cliente envía una solicitud HTTP a la API para crear un nuevo recurso.
2. **Procesamiento en la API**:
   - Se crea una nueva entidad en la base de datos.
   - Se genera un evento asociado y se almacena en una tabla "outbox" dentro de la misma transacción de base de datos.
3. **Procesamiento en Segundo Plano**:
   - Un servicio en segundo plano monitorea la tabla "outbox" en busca de eventos pendientes.
   - Al detectar un nuevo evento, el servicio lo publica en el sistema de mensajería.
   - Si la publicación es exitosa, el evento se marca como procesado; de lo contrario, se reintentará en el siguiente ciclo.

Este enfoque asegura la consistencia entre la base de datos y los mensajes enviados a otros servicios.

## Patrón Backend for Frontend (BFF)

El patrón Backend for Frontend (BFF) proporciona un back-end optimizado para cada tipo de front-end. En este caso, `Sample.Front` actúa como intermediario entre el front-end y los servicios back-end, ofreciendo beneficios como:

- **Optimización de Respuestas**: Adaptando las respuestas según las necesidades del front-end.
- **Simplicidad en el Mantenimiento**: Separando la lógica del front-end de los servicios internos.
- **Seguridad Mejorada**: Centralizando la autenticación y reduciendo la exposición de los servicios back-end.

## Autenticación y Seguridad

El proyecto utiliza **autenticación basada en tokens JWT (JSON Web Tokens)**, gestionada por el BFF. Este enfoque mejora la seguridad al:

- **Reducir la Superficie de Ataque**: El front-end se comunica solo con el BFF, sin acceso directo a los servicios internos.
- **Gestionar Sesiones de Forma Centralizada**: Controlando la expiración y revocación de tokens.
- **Implementar Medidas Adicionales**: Como protección contra ataques de fuerza bruta y validación de entradas.

## Contribuciones

Las contribuciones son bienvenidas. Si encuentras algún problema o tienes sugerencias, abre una "issue" o envía una "pull request".

## Licencia

Este proyecto está bajo la Licencia MIT.


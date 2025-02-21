# SampleApiWithOutbox

Este repo es un ejemplo de cómo **yo** creo que hay que organizar una aplicación web, centrandome en el backend. 

Se compone de un frontal (*Sample.Front*) que llama a una API interna mediante el patrón Back-For-Front. Es decir, la parte servidora del front hace de proxy a la API del back (*Sample.API*).
La autenticación se hace contra un IDP (incluido en la solución por comodidad, proyecto *Idsrv4*). De cara al usuario, la autenticación se hace mediante cookies (*Strict-Same-Site*) y hacia la API del back usa el flujo *Machine-to-Machine*.

# Funcionalidad

Se trata de un mantenimiento de mensajes, como podrían ser los SMS. Es una excusa para representar el modelado orientado a Dominio en capas (proyectos *Sample.Domain*, *Sample.Application* y *Sample.Infrastructure*).

Cuando se procesa un mensaje, si contiene cierta palabra, se lanza un evento que es almacenado en un outbox para mantener la transaccionalidad en la base de datos. Luego, otro proceso va leyendo los mensajes del outbox y los entrega a los suscriptores (patrón Outbox).
Los subscriptores solo están interesados en el evento *MatchingMessageReceived*, que  como ya se ha dicho es lanzado por el método *Process* del agregado *Message*.

# Capas

## Dominio
Esta capa es la más protegida y por ello no depende de ningún otro proyecto.
Aquí se definen los eventos y agregados de nuestro dominio (en este caso *MatchingMessageReceived* y *Message*), así como las interfaces necesarias cuya implementación se delega en otras capas (p.ej, *IMessageRepository*). 

## Aplicación
Esta capa tiene dependencia de la capa de dominio pero nada más. Aquí se implementan los comandos que, usando las implementaciones de la capa de infraestructura, realizan los casos de uso y reaccionan a los eventos.

## Infraestructura
Esta capa tiene dependencia de la capa de dominio, pero no de la de aplicación. Se implementan los repositorios, clientes de service bus, proxys a otras apis... Cualquier cosa que dependa de una tecnología concreta. *Casi* siempre se trata de dependencias out-of-process. 


# Tests

No hay tests del proyecto Front.
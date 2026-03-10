# 🏦 Sistema Sistema Tarjetas Banco ABC


**Institución**

- Colegio Universitario de Cartago (CUC)

**Curso**

- TI-151 Programación IV

**Grupo**

- 01

**Periodo**

- I Cuatrimestre, 2026

**👥 Participantes**

- Nestor Leiva
- Rodrigo Elias 

**Profesor(a)**

- Milena Soto


Este proyecto es un ecosistema bancario distribuido que simula el flujo real de transacciones de un ATM, desarrollado para el curso TI-151 Programación IV (CUC). La arquitectura demuestra la interoperabilidad entre tres lenguajes y el manejo de estados críticos.

## 🏗️ Evolución del Proyecto

El sistema se ha desarrollado en dos fases incrementales para demostrar diferentes modelos de comunicación:

### 🔹 Avance 1: Conexión Directa (Socket-Based)


-   **Gestión de Sesión de Cajero:**  Al iniciar, el simulador permite configurar dinámicamente el `id_cajero`, validando su existencia en la base de datos MySQL antes de permitir operaciones.
    
-   **Protocolo de Trama Híbrida:** * **Tramas de Texto Plano:** Implementación de mensajes de longitud fija para **Retiro** y **Consulta**, optimizando el ancho de banda de red.
    
    -   **Serialización JSON Manual:** Implementación de objetos JSON construidos manualmente en C# para el **Cambio de PIN**, permitiendo flexibilidad en el envío de datos complejos sin dependencias externas.
        
-   **Validación de Credenciales:** El sistema realiza un "Triple Check":
    
    1.  Existencia de la tarjeta.
    2.  Correspondencia del PIN.
    3.  Fecha de vencimiento y CVV.
        
-   **Interoperabilidad Multi-Lenguaje:** Comunicación exitosa de tipos de datos entre C# (Little Endian) y Python, garantizando que los montos y IDs se procesen sin corrupción de memoria.
    

### 📊 Especificación de Mensajes (Avance 1)
| Operación | Formato de Envío | Estructura de Datos
|:------------:|:---------------------:|:---------------------:|
**Consulta** |Trama Fija (Tipo 2) |`2` + `Tarjeta(16)` + `00000000` + `PIN(4)` + `Cajero(4)`|
**Retiro** |Trama Fija (Tipo 1) |`1` + `Tarjeta(16)` + `Monto(8)` + `PIN(4)` + `Cajero(4)`|
**Cambio PIN** |**JSON Estructurado** | `{"tipo": "cambio_pin", "numero_tarjeta": "...", "pin_actual": "...", ...}`


### 🔹 Avance 2: Arquitectura Orientada a Servicios (SOA)

Se introdujo una capa intermedia de servicios para estandarizar la comunicación:

**_WCF_** (Windows Communication Foundation): El Simulador ya no habla con Python directamente, sino que consume un Web Service (SOAP) hospedado en el WCF.
**_Interoperabilidad_**: El WCF actúa como puente, recibiendo peticiones del cliente C# y traduciéndolas a tramas/JSON para el servidor Python.
**_Seguridad_**  : Centralización de la lógica de encriptación y validación en el Service Layer.

Componente, Lenguaje, Base de Datos, Responsabilidad

Simulador, .NET / C# (Consola), N/A, Interfaz de usuario y consumo de WCF.

Web Service, WCF (.NET), N/A, "Capa de servicio, lógica de negocio y proxy de red."

Autorizador, Python 3.11+, MySQL, "Validación de PIN, Gestión de Bitácora y Reglas."

Core, Java 17 (JDK), SQL Server, Libro mayor contable y procesamiento de saldos.

### 📋 Funcionalidades Implementadas

**_WS_AUTORIZADOR1_**  (Retiro): Sincronización de saldos en tiempo real entre MySQL y SQL Server.

**_WS_AUTORIZADOR2_**  (Consulta): Recuperación de montos actuales desde el Core Java con formato de moneda (₡).

**_WS_AUTORIZADOR3_** (Cambio de PIN): Actualización segura de credenciales mediante objetos JSON enviados desde el WCF al socket Python.

AUT4 (Auditoría): Registro asíncrono de eventos en bitacora_4.txt con enmascaramiento de datos sensibles.

### 🚀 Guía de Ejecución (Orden Requerido)

Para que los sockets y servicios se reconozcan entre sí  inicie en este orden:

	Core Java (Puerto 5000): terminal

		java com.core. CoreBancario

	Autorizador Python (Puerto 5001): Terminal
		python main.py

	WCF Service:  
		Abrir el proyecto WS_AUTORIZADOR en Visual Studio y ejecutar (F5) para levantar el servidor de servicios.

	Simulador ATM (C#): 
		Ejecutar el proyecto de consola que consume el servicio WCF.

#### 📊 Protocolos de Comunicación

1. Comunicación WCF ↔ Python (Socket)

El WCF envía dos tipos de paquetes al puerto 5001:

Trama Plana (Retiro/Consulta): [Tipo:1][Tarjeta:16][Monto:8][PIN:4][Cajero:4]

**Objeto JSON (Cambio PIN):**

	JSON

		{
			"tipo": "cambio_pin",
			"numero_tarjeta": "4111...",
			"pin_actual": "1515",
			"pin_nuevo": "1234",
			"id_cajero": 1
		}

2. Comunicación Python ↔ Java (Socket)

Para afectar el saldo maestro en SQL Server:

[Tipo:1][Cuenta:23][Tarjeta:18][CodAuth:8][Monto:8]

🛡️ Medidas de Seguridad Implementadas

**Enmascaramiento**: Los números de tarjeta en los logs se muestran como 4111********1111.

**Aislamiento de Datos**: El cliente nunca tiene acceso directo a las bases de datos; toda petición pasa por el WCF y el Autorizador.

**Validación Cruzada**: Un retiro solo se confirma si el Core Java (SQL) y el Autorizador (MySQL) responden exitosamente (Doble Commit).
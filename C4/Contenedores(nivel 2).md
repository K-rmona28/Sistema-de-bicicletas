## **Nivel 2: Contenedores del sistema

```mermaid
C4Context

title Contenedores del sistema 

Person(usuario, "Cliente final")
Person(admin, "Administradores Globales y Subordinados")


System_Ext(psp, "Pasarela De Pagos")
System_Ext(correo, "Servicio de Correo")
System_Ext(wa, "WhatsApp")
System_Ext(noti, "Sistema de Notificaciones Push")
System_Ext(gps, "Servicio de localizacion/Gps")



Container(app, "Aplicacion Web","Angular Propuesto", "Interfaz de usuario y administracion con multilenguaje en vivo")
Container(api, "Api Backend","ASP.Core Web Api Propuesto","Reglas de negocio y logica del sistema, administracion en tiempo real")


ContainerDb(db, "Base de datos Operativa", "SQl Server Propuesto", "Usuarios,Prestamos,Estaciones,Bicicletas,Pagos,Sanciones,Reportes,Reservas")
ContainerDb(auditoria, "Almacen de Auditoria", "Tablas only-append Propuesto","Registro Inmutable de Registros")
ContainerDb(eviden, "Almacenamiento de Evidencias", "Archivos Propuesto", "Evidencias de reportes inmutable")


Rel(usuario, app, "Usa", "HTTPS")
Rel(admin, app, "Usa", "HTTPS")
Rel(api,wa, "Envia Y Recibe Mensajes", "HTTPS")
Rel(api,noti, "Envia Notificaciones", "HTTPS")
Rel(api,gps, "Consultar Ubicacion", "HTTPS")
Rel(api,correo, "Envio de Correos", "API")
Rel(api,psp, "Procesa Cobros", "HTTPS")
Rel(app,api, "Consume API REST", "HTTPS/JSON")
Rel(api,db, "Registrar y leer", "SQL")
Rel(api,auditoria, "Solo inserta, lee y escribe evidencias", "SQL")
Rel(api,eviden, "Inserta Automaticamente", "SQL")




```

| Contenedor | Tecnología (propuesta) | Responsabilidad | Impulsores |
|---|---|---|---|
| Aplicación Web (SPA) | Angular | Vistas de cliente y de administración, cambio de idioma, contador de tiempo restante sin recargar, ocultar funciones no autorizadas | RNF-13, RNF-20, RNF-23, HU-21 |
| API Backend | ASP.NET Core Web API en capas | Casos de uso, reglas de negocio, RBAC, canal en tiempo real, integración con pasarela, GPS y notificaciones, logs, y tareas programadas (préstamos vencidos y sanciones, avisos de tiempo restante, saturación de estaciones) | RNF-01 a RNF-03, RNF-08, RNF-11, RNF-12, RNF-16 a RNF-18, RN-06, RN-028 |
| Base de datos operativa | SQL Server | Datos del negocio: usuarios, estaciones, bicicletas, reservas, préstamos, pagos, sanciones y reportes. El backup periódico se configura en el motor | RNF-05 a RNF-07, HU-19 |
| Almacén de auditoría | Tablas append-only (esquema o base separada, solo INSERT) | Registro inmutable de cambios, sanciones, permisos y accesos a datos personales | RN-09, RNF-09, RNF-21, RNF-22 |
| Almacenamiento de evidencias | Archivos / Blob | Evidencias adjuntas a reportes; una vez subidas no se modifican | RN-022 |
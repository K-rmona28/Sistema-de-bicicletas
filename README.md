# 🚴‍♂️ Sistema de Bicicletas

## 📘 Descripción
Este repositorio fue creado con el propósito de **organizar y visualizar de una mejor manera** la información relacionada con los **requisitos funcionales, no funcionales y las historias de usuario** del proyecto académico **Sistema de Bicicletas**.  
El objetivo es contar con un espacio centralizado y claro donde se pueda consultar el backlog completo y la documentación del sistema.

---

## 🧩 Objetivos
- Facilitar el acceso a los requisitos funcionales y no funcionales.  
- Mantener un backlog estructurado con las 21 historias de usuario refinadas.  
- Proporcionar una base clara para el desarrollo backend y la gestión del proyecto.  
- Garantizar trazabilidad y transparencia en la documentación.

---

## 🧠 Alcance Funcional
El sistema contempla los siguientes módulos principales:

| Módulo | Descripción |
|--------|-------------|
| Autenticación y Usuarios | Registro, login y recuperación de contraseña. |
| Bicicletas y Estaciones | Consulta, reserva y redistribución de bicicletas. |
| Préstamos y Costos | Registro de operaciones y cálculo de tarifas. |
| Notificaciones | Alertas push, correo y WhatsApp. |
| Reportes y Mantenimiento | Registro y gestión de daños o pérdidas. |
| Roles y Permisos | Control de acceso basado en roles (RBAC). |
| Auditoría y Seguridad | Registro inmutable de operaciones y cumplimiento legal. |

---

## 🧾 Requisitos Funcionales
El backlog completo con las **21 Historias de Usuario refinadas** está disponible en el [Project Backlog](./projects).  
Cada historia incluye:
- Contexto y valor de negocio.  
- Criterios de aceptación en formato Gherkin.  
- Reglas de negocio y requisitos no funcionales asociados.  
- Datos de prueba y criterios DoR/DoD.

---

## ⚙️ Requisitos No Funcionales
El sistema cumple con los siguientes RNF clave:
- **Performance:** Respuesta menor a 2 s en consultas y 5 s en actualizaciones.  
- **Disponibilidad:** 90 % mensual, con backups automáticos.  
- **Seguridad:** Cifrado de datos personales y control RBAC.  
- **Usabilidad:** Interfaz intuitiva y multilenguaje.  
- **Escalabilidad:** Capacidad para incorporar nuevas estaciones y usuarios.  
- **Legalidad:** Cumplimiento de la Ley 1581 de 2012 (Habeas Data).

---

## 🧱 Arquitectura Propuesta
El sistema se desarrollará bajo una arquitectura **cliente‑servidor** con enfoque **API RESTful**:


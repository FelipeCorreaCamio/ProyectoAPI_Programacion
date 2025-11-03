# **PharmaLink — API REST Farmacia**

## **Objetivo**
Desarrollar una **API REST** para la gestión de **medicamentos**, **dispensaciones** y **envío de insumos médicos** al hospital.  
La API deberá **integrarse con la del hospital** para **validar recetas electrónicas** y **atender las solicitudes de reposición**.

---

## **Requerimientos funcionales**

### **1. Gestión de medicamentos e insumos**
El sistema deberá permitir:
- Registrar, consultar y actualizar medicamentos e insumos médicos.
- Incluir: **nombre**, **presentación**, **stock disponible** y **precio unitario**.

### **2. Gestión de dispensaciones**
El sistema deberá permitir registrar la **entrega de medicamentos** en base a una receta válida emitida por el hospital.  
Cada dispensación deberá incluir:
- Código de receta  
- Medicamento entregado  
- Cantidad  
- Fecha de entrega  

### **3. Validación de recetas**
Antes de dispensar, el sistema deberá:
- Validar la receta consultando la **API del hospital**.  
- Si la receta es **válida y está activa**, podrá registrarse la entrega y actualizar el stock.

### **4. Gestión de pedidos de reposición**
El sistema deberá:
- Recibir **solicitudes de reposición** provenientes del hospital.  
- Validar la **disponibilidad de insumos**.  
- Generar la **respuesta de envío** correspondiente.  
- Una vez confirmada, **actualizar el stock** y registrar la entrega.

### **5. Autenticación y control de acceso**
- Las operaciones de **validación**, **dispensación** y **reposición** deberán requerir **autenticación mediante API Key**.  
- Todas las respuestas deberán seguir un **formato estándar**.

---

## **Criterios de aceptación**
- No se podrá dispensar una receta sin validarla en la API del hospital.  
- Cada entrega de medicamentos o insumos deberá registrar **fecha, cantidad y confirmación de envío**.  
- Las solicitudes de reposición deberán **actualizar el stock correctamente**.  
- Todas las operaciones deberán devolver **respuestas JSON** y **códigos HTTP adecuados**.

---

## **Consideraciones técnicas**
- Autenticación mediante **API Key** en encabezados.  
- Validación de datos y manejo uniforme de errores en formato:  
  ```json
  {
    "code": "error_code",
    "message": "Descripción del error",
    "details": "Información adicional"
  }
  ```

---

## **Entregables**
- Código fuente de la **API RESTful**.  
- Modelo de la base de datos para:
  - **Medicamentos**
  - **Dispensaciones**
  - **Insumos**
- Documentación de los **endpoints principales** de *PharmaLink*, incluyendo:
  - URLs  
  - Métodos  
  - Parámetros requeridos  
  - Ejemplos de respuesta  

---

## **Desafío adicional**
Implementar una **vista combinada** que muestre el flujo completo de reposición:
- Solicitudes recibidas del hospital  
- Confirmaciones enviadas  
- Stock actualizado de cada insumo  

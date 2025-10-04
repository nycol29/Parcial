## Pregunta 5 — Panel Coordinador

### Objetivo
Permitir que los usuarios con rol **Coordinador** gestionen cursos y matrículas.

### Funcionalidades
- CRUD de cursos:
  - Crear
  - Editar
  - Desactivar
- Listado de matrículas por curso con opciones:
  - Confirmar
  - Cancelar
- Solo usuarios con rol Coordinador pueden acceder al panel y ver botones CRUD.
- Mensajes de éxito/desactivación usando `TempData`.

### Rutas principales
- `/Coordinador/Cursos` → Lista de cursos
- `/Coordinador/Create` → Crear curso
- `/Coordinador/Edit/{id}` → Editar curso
- `/Coordinador/Desactivar/{id}` → Desactivar curso
- `/Coordinador/Matriculas/{cursoId}` → Ver matrículas de un curso

### Configuración local
1. Clonar el repositorio:
   ```bash
   git clone <URL_DEL_REPO>
   cd PortalAcademico

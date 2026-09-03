# SpellCardDev
> Laboratorio de desarrollo Danmaku y Spell Cards en Unity (C#).  
> Arquitectura orientada a rendimiento, cálculo geométrico y control de proyectiles.

---

## 1. Arquitectura de Scripts (C#)

El proyecto separa la lógica en módulos independientes dentro de `Assets/_Project/Scripts/`:

| Módulo | Script | Responsabilidad |
| :--- | :--- | :--- |
| **Player** | `PlayerController.cs` | Movimiento en 8 direcciones, modo Focus, control de vidas (3), invulnerabilidad temporal y HUD. |
| | `PlayerShooter.cs` | Disparo continuo primario (`Z`) y ataque cargado con indicador visual (`X`). |
| | `PlayerBomb.cs` | Bomba de onda expansiva circular (`C`) que barre balas enemigas en tiempo real y daña jefes. |
| **Enemies** | `BossController.cs` | Secuencia de presentación (entrada suave), barra de HP superior, fases y pantalla de reintento. |
| | `FairyEnemy.cs` | Ciclo de vida completo: trayectoria de entrada, flotado con ataque configurable y retirada. |
| | `FairySpawner.cs` | Gestor de oleadas periódicas con diferentes patrones de disparo (abanicos, ráfagas, anillos). |
| | `CirclePatternSpawner.cs` | Generador de anillos radiales equidistantes de 360° con rotación incremental opcional. |
| **Bullets** | `BulletPool.cs` | Pool de objetos preasignado (`Queue<GameObject>`) para reciclaje sin recolección de basura. |
| | `EnemyBullet.cs` | Proyectil enemigo con rotación automática a la trayectoria, override de sprite y hitbox dinámico. |
| | `PlayerBullet.cs` | Proyectil del jugador con detección de impacto contra jefes y hadas. |
| **Background**| `BackgroundVideoPlayer.cs` | Gestor de color de fondo sólido (azul oscuro/índigo cósmico `#0D0F1A`) con vista previa en editor. |

---

## 2. Notas Técnicas & Rendimiento (C# en Unity)

### Gestión de Memoria (Zero-Allocation en combate)
* **Object Pooling Obligatorio:** Nunca invocar `Instantiate()` ni `Destroy()` durante el gameplay para proyectiles. Todo ciclo de vida pasa por `BulletPool.GetBullet()` y `BulletPool.ReturnBullet()`.
* **Caché Estático de Sprites:** Los proyectiles usan referencias cacheadas para evitar regenerar texturas en tiempo de ejecución.
* **Corrutinas Limpias:** Cada corrutina de disparo o flash visual valida referencias y se detiene explícitamente al salir de escena o morir la entidad.

### Detección de Colisiones 2D
* **Física Cinemática:** Entidades y balas operan con `Rigidbody2D` en modo `Kinematic` con `gravityScale = 0` y `CollisionDetectionMode2D.Continuous`.
* **Hitbox del Jugador:** Radio diminuto (`0.06f`) fiel al estándar Touhou. El indicador visual solo se activa al sostener la tecla de Focus (`Shift`).
* **Hitbox Dinámico de Proyectiles (PPU 32):** El colisionador calcula su radio en base a las dimensiones en píxeles del sprite asignado:
  $$\text{Radius} = \left(\frac{\min(\text{width}, \text{height})}{2 \times \text{PPU}}\right) \times \text{hitboxRatio}$$
  Permite intercambiar balas de 8x8, 16x16 o 32x32 manteniendo la colisión proporcional automáticamente.

---

## 3. Fórmulas de Patrones Danmaku

```csharp
// 1. Anillo Equidistante (360°)
float angleStep = 360f / bulletCount;
float angleRad = (baseRotation + i * angleStep) * Mathf.Deg2Rad;
Vector2 dir = new Vector2(Mathf.Sin(angleRad), Mathf.Cos(angleRad));

// 2. Disparo Dirigido (Aimed to Player)
Vector2 toPlayer = (playerTransform.position - origin.position).normalized;

// 3. Abanico Angular (Spread / Fan)
float halfSpread = spreadAngle * 0.5f;
float angle = Mathf.Lerp(-halfSpread, halfSpread, (float)i / (count - 1));
Vector2 fanDir = Quaternion.Euler(0f, 0f, angle) * toPlayer;
```

---

## 4. Mapeo de Controles

| Entrada | Acción | Detalle |
| :--- | :--- | :--- |
| **Flechas** | Movimiento | 8 direcciones normalizadas (velocidad constante en diagonales). |
| **Shift Izq** | Modo Focus | Reduce velocidad a `2.5` y hace visible el punto de colisión central. |
| **Z** | Disparo | Fuego rápido continuo con balas ascendentes. |
| **X (Mantener)**| Carga | Carga energía con aura visual; al soltar lanza abanico de hasta 7 balas. |
| **C** | Bomba | Expande un anillo de barrido que elimina balas en contacto (Cooldown: 4s). |

---

## 5. Parámetros Globales

* **Resolución base:** `1280 x 960` (Aspecto 4:3).
* **Cámara:** Ortográfica, `Size = 8.0` (16 unidades de alto).
* **PPU Estándar:** `32` para todos los assets de sprites del juego.

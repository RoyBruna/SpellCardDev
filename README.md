```text
  ___ ___ ___ _    _     ___   _   ___ ___    ___  _____   __
 / __| _ \ __| |  | |   / __| /_\ | _ \   \  |   \| __\ \ / /
 \__ \  _/ _|| |__| |__| (__ / _ \|   / |) | | |) | _| \ V / 
 |___/_| |___|____|____|\___/_/ \_\_|_\___/  |___/|___| \_/  
=============================================================
      [ DANMAKU DEVELOPMENT LAB // SPELL CARD WORKSHOP ]
=============================================================
```

> *"Las reglas de las Spell Cards no fueron creadas para destruir,*  
> *sino para transformar el combate en una obra de arte geometrico y caotico."*

---

## [ 01 ] Proposito del Espacio

Este repositorio es un laboratorio de practica dedicado exclusivamente al diseno, calculo y programacion de patrones de proyectiles (**danmaku**) y **Spell Cards** inspiradas en el universo de *Touhou Project*.

El objetivo central es dominar la arquitectura y la matematica detras de la generacion de patrones:
- **Trigonometria y sistemas polares:** Conversion fluida de coordenadas cartesianas a polares y viceversa.
- **Curvas parametricas y distribuciones angulares:** Funciones matematicas para modelar formas de flores, lazos y trayectorias no lineales.
- **Dinamicas de proyectil:** Modulacion de velocidad ($v$), aceleracion ($a$), arrastre y curvas angulares en tiempo real.
- **Maquinas de estado y secuenciadores:** Control de fases, timers y transiciones de patrones complejos.
- **Arquitectura de rendimiento:** Implementacion de *Bullet Pooling* y estructuras eficientes para manejar miles de objetos simultaneos a 60 FPS estables.

---

## [ 02 ] Fundamentos y Matematica de Danmaku

El diseno de patrones no depende de la aleatoriedad, sino de geometria precisa en movimiento.

```text
                       (0, -r)
                          |  ^ dy = -sin(theta) * v
                          |  |
             (-r, 0) -----+----- (r, 0)
                          |  |
                          |  v dy =  sin(theta) * v
                       (0,  r)
               dx = cos(theta) * v
```

### Formulas Clave

```text
+----------------------+----------------------------------------------------------+
| Concepto             | Formula / Algoritmo                                      |
+----------------------+----------------------------------------------------------+
| Ring Spread          | theta_n = base_angle + (n * (2 * PI / count))            |
| Targeted (Aiming)    | angle = atan2(player.y - origin.y, player.x - origin.x)  |
| Spiral Stream        | theta(t) = (t * delta_angle) % (2 * PI)                  |
| Rose Curve (Flores)  | r(theta) = a * cos(k * theta)                            |
| Trayectoria Variable | v(t) = v_0 + a * t ; theta(t) = theta_0 + omega * t      |
+----------------------+----------------------------------------------------------+
```

---

## [ 03 ] Arquitectura de una Spell Card

```text
      +--------------------------------------------------+
      |                 SPELL CARD ENGINE                |
      +--------------------------------------------------+
                               |
            +------------------+------------------+
            |                                     |
            v                                     v
   +-----------------+                   +-----------------+
   |    TIMELINE     |                   |   BULLET POOL   |
   | (State Machine) |                   | (Pre-allocated) |
   +-----------------+                   +-----------------+
            |                                     |
            +------------------+------------------+
                               |
                               v
                      +-----------------+
                      |    EMITTERS     |
                      | (Ring, Spiral,  |
                      | Laser, Curving) |
                      +-----------------+
                               |
                               v
                      +-----------------+
                      | PROJECTILE SIM  |
                      | (Step / Render) |
                      +-----------------+
```

---

## [ 04 ] Laboratorio de Patrones (Roadmap de Practica)

### Fase 1: Emisiones Fundamentales
- [ ] **Anillo Simple Equidistante:** Generacion de $N$ proyectiles distribuidos simetricamente en $360^\circ$.
- [ ] **Anillos Concentricos con Desfase:** Capas con diferentes velocidades y rotacion angular intercalada.
- [ ] **Cono Dirigido (Odd & Even):** Dispersiones apuntadas al objetivo calculando offsets para cantidad par e impar de balas.

### Fase 2: Rotaciones y Espirales
- [ ] **Espiral de Arquimedes:** Emision continua con incremento constante de angulo.
- [ ] **Doble Espiral Cruzada:** Dos emisores coaxiales rotando en sentidos opuestos.
- [ ] **Molinete Multi-Brazo (Pinwheel):** $N$ ramas rotatorias con aceleracion variable.

### Fase 3: Dinamicas Parametricas y Transformaciones
- [ ] **Balas Ondulatorias:** Proyectiles con oscilacion transversal sinusoidal (*wobble*).
- [ ] **Freno y Redireccion (Stop & Go):** Desaceleracion a $v=0$, pausa configurable y aceleracion en nuevo angulo.
- [ ] **Patron Flor Polar:** Curvas de rosa ($r = a \cdot \sin(k \theta)$) expandiendose radialmente.

### Fase 4: Cortinas Compuestas y Sub-Balas
- [ ] **Cortina de Lluvia Geometrica:** Generacion matricial con carriles de paso predecibles.
- [ ] **Burst Secundario:** Proyectiles principales que detonan en sub-anillos al expirar su tiempo de vida.
- [ ] **Flor de Loto:** Expansion radial inicial seguida de contraccion o cambio de trayectoria invertida.

### Fase 5: Spell Cards Completas (Boss Phases)
- [ ] **Spell Card 01:** Patron dual sincronizado con fases de carga (*charge-up*) y disparo sostenido.
- [ ] **Spell Card 02:** Patron asimetrico disenado para micro-esquiva (*micro-dodging / streaming*).
- [ ] **Spell Card 03:** Danmaku estetico de dos capas (malla de fondo lenta + lasers directos).

---

## [ 05 ] Principios de Diseno ZUN-Style

1. **La belleza visual es la prioridad:**  
   Un patron debe verse hipnotico y legible. La pantalla llena de balas debe parecer una pintura matematica antes que caos puro.

2. **Esquivabilidad Justa (Fair Dodging):**  
   Todo patron tiene una logica de escape clara (*streaming*, macro-esquiva, micro-esquiva o *grazing*). Un impacto nunca debe ser producto de un RNG injusto.

3. **Hitbox y Colision Rigurosa:**  
   El radio de colision de cada proyectil debe ser diminuto y perfectamente centrado respecto al sprite.

4. **Rendimiento Impecable:**  
   60 FPS obligatorios. El pooling riguroso de objetos y calculos directos en memoria garantizan fluidez absoluta incluso con miles de proyectiles activos.

---

```text
+-------------------------------------------------------------+
|               "MAKE THE SCREEN BLOOM."                      |
+-------------------------------------------------------------+
```

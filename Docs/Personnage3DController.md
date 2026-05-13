# Contrôleur de personnage 3D Unity 6

Ce contrôleur utilise `CharacterController` et l'API d'entrée historique de Unity, disponible dans Unity 6 sans dépendance supplémentaire.

## Installation

1. Créez un GameObject `Player` dans votre scène.
2. Ajoutez-lui un composant `CharacterController`.
3. Ajoutez le script `Personnage3DController` sur le même GameObject.
4. Placez la caméra principale dans la scène et assignez-la au champ `Camera Transform`, ou laissez le champ vide pour utiliser automatiquement `Camera.main`.
5. Ajustez `Vitesse Marche`, `Hauteur Saut`, `Distance Camera` et `Hauteur Camera` depuis l'inspecteur.

## Contrôles

- `Z` / flèche haut : avancer.
- `Q` / flèche gauche : aller à gauche.
- `S` / flèche bas : reculer.
- `D` / flèche droite : aller à droite.
- `Espace` : sauter.
- Souris : orienter la caméra derrière le personnage.
- `Échap` : libérer le curseur.
- Clic gauche : reverrouiller le curseur.

## Notes de compatibilité Unity 6

Le script est autonome et ne requiert pas le package Input System. Si votre projet utilise uniquement le nouveau Input System, activez `Both` ou `Input Manager (Old)` dans **Project Settings > Player > Active Input Handling** pour conserver ces contrôles ZQSD tels quels.

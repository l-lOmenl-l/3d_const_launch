# Parallax.WPF
Parallax Effect for WPF
## Forked from https://github.com/LiptonOlolo/ParallaxEffect

* After you installed Parallax.WPF install `Install-Package DevExpressMvvm -Version 18.2.3`
* Add reference : 
```XAML
xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
xmlns:parallax="clr-namespace:Parallax.WPF;assembly=Parallax.WPF"
```
* Use the effect like this :
```XAML
<Image Source="Images/Backgrounds/cosmo_bg.PNG" Margin="-30" Stretch="UniformToFill"
                     parallax:ParallaxEffect.IsBackground="True"
                     parallax:ParallaxEffect.Parent="{Binding ElementName=TopGrid}"
                     parallax:ParallaxEffect.UseParallax="True"
                     parallax:ParallaxEffect.XOffset="50"
                     parallax:ParallaxEffect.YOffset="50">
    <i:Interaction.Behaviors>
        <parallax:ParallaxEffect/>
    </i:Interaction.Behaviors>
</Image>
```

## Demo 

https://i.imgur.com/Ikm9vrF.gif


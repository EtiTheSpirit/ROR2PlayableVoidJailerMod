## 1.3.0
<details open>
<summary><strong>"please dont delete me with the void i have phantom sense" haha jailer go BFMMMVMMVM</strong></summary>
<br/>
<ul>
<li>Added experimental VR support through VRAPI. A few UX issues here and there but they are mostly ironed out.</li>
<li>Added a new configuration option to do further aim compensation whilst in VR. It feels better to play with this on, but it <em>technically</em> gives you a slight advantage (albeit minimal). Configure at your own discretion.</li>
<li>The wind-down state of Bind (after grabbing something) can be interrupted by standard skills now, making combos a little faster to pull off.</li>
<li>Fixed a bug causing the lasso effect spawned by Bind to originate from your torso instead of from the muzzle of the arm.</li>
</details>

## 1.2.2
<details>
<summary><strong>haha spike no longer go GGFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFHKHHHHKHHKHHHHKHKHKKCHKKKHHHKKHKHHJHKHVCJHJHHHJCKHHKCHHKKK :DDD</strong></summary>
<br/>
<ul>
<li>Perforate (special) no longer gets "stuck" for a second or so when activating it whilst firing Spike or Bind; it now interrupts those abilities and activates immediately. No more wasted shots due to an annoying delay.</li>
<li>Fixed an issue where Spike could interrupt itself, spamming the charge-up sound when holding the primary fire button (it now only gets interrupted when frozen or killed, or by a priority skill).</li>
<li>Fixed an issue where Perforate could interrupt itself, firing darts every frame instead of at the appropriate interval (it now only gets interrupted when frozen or killed).</li>
<li>Fixed an issue where Bind could be interrupted in the middle of its sequence by the player (it now only gets interrupted when frozen or killed).</li>
</details>

## 1.2.1
<details>
<summary><strong>A minor tweak while I proceed to do everything <em>except</em> work on this mod. I'm working on other stuff. Sorry.</strong></summary>
<br/>
<ul>
<li>Significantly buffed the damage that Spike/Perforate does to Nullified targets, to better encourage using combos with the secondary.</li>
<li>The damage boost on Nullified targets is now a configuration option, and its default has been increased to +125% (from +70%).</li>
<li>Buffed the default lifesteal done by Bind to 20% (from 5%). <strong>⚠ Existing users will need to make this change themselves via the configuration file.</strong></li>
<li>Buffed the duration of the Nullify effect done by Bind (Normal enemies: 6s => 10s, Bosses: 3s => 5s). <strong>⚠ Existing users will need to make this change themselves via the configuration file.</strong></li>
</details>

## 1.2.0
<details>
<summary><strong>Some changes based on user feedback and some additional features.</strong></summary>
<ul>
<li>Added <a href="https://thunderstore.io/package/Rune580/Risk_Of_Options/">Risk Of Options</a> as a dependency.</li>
<li>The camera offset can now be configured. <strong>You can control this in real time with Risk of Options in-game.</strong></li>
<li>The character now becomes slightly transparent in combat. You can configure the transparency in and out of combat in the mod's configs. <strong>You can control this in real time with Risk of Options in-game.</strong></li>
<li>The default camera position is no longer set up like a shoulder camera.</li>
<li>Added <a href="https://thunderstore.io/package/Xan/NoSelfPing/">NoSelfPing</a> as a dependency to resolve the issue with being unable to ping things. It's part of why I actually made NoSelfPing in the first place.</li>
<li>Increased the hitscan angle and the maximum distance of Bind to make hits feel much more consistent and work from a longer range.</li>
<li>Bind now saps health from the target. The amount has been added to the mod's configuration. Its default value is 5%.</li>
<li>Bind now has a configurable duration for its application of Nullify. There is a separate configuration for bosses to balance it against bosses.</li>
<li>Fury now replaces Spike with a new sub-skill called "Perforate" that makes you fire much faster, but with less projectiles per burst. The net damage per second is much higher, and it becomes easier to deal with swarms. Spray and pray.</li>
<li>Some settings have had their defaults changed. It is recommended that you review your settings if you want to try the new defaults.</li>
</ul>
</details>

## 1.1.0
<details>
<summary><strong>Okay NOW it works. I hope.</strong></summary>
<ul>
<li>Fixed a few foolish bugs that could completely brick visual effects.</li>
<li>Fixed an error causing the Jailer to be unusable (stuck, could not activate skills) on remote clients in multiplayer environments.</li>
<li>Fixed a few bugs relating to missing networking components on objects that needed them.</li>
<li>Changed stats to be more appropriate.</li>
</ul>
</details>

## 1.0.1
<details>
<summary><strong>Yeah, I guess that was my fault.</strong></summary>
<ul>
<li>Fixed a reflection exception preventing the mod from loading on some clients.</li>
</ul>
</details>

## 1.0.0
<details>
<summary><strong><em>"Go feed a bottom you bottom feeder"?</em> What is that supposed to mean?</strong></summary>
<ul>
<li>ourple lober gaming</li>
</ul>
</details>

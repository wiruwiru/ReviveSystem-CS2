# ReviveSystem CS2
> Allows players to revive one of their teammates per round by holding “E” on the dead teammate.. (for CounterStrikeSharp)

## ⚙️ Installation
1. Install [CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp) and [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master).

2. Download [ReviveSystem.zip](https://github.com/wiruwiru/ReviveSystem-CS2/releases) from the releases section.

3. Unzip the archive and upload it to the game server.

4. Start the server and wait for the `config.json` file to be generated.

5. Complete the configuration file with the parameters of your choice.

## 📁 Configuration
The configuration file (`ReviveSystem.json`) will be auto-generated after the first launch.

## 📜 Lang Configuration
In the 'lang' folder, you'll find various files. For instance, 'es.json' is designated for the Spanish language. You're welcome to modify this file to better suit your style and language preferences. The language utilized depends on your settings in 'core.json' of CounterStrikeSharp.

## TODO
| Task | Status | Description |
|------|--------|-------------|
| Hold "E" to revive | Complete | Implement the mechanic where the player must hold the "E" key over a body to revive a teammate. |
| Display revival timer in HUD | Complete | Show a countdown timer in the HUD indicating how much time is left for the revival process. |
| Limit revives to one per round | Complete | Ensure a player can only revive one teammate per round. |
| Optimize and improve plugin logic | Pending | Optimize and enhance the plugin's logic to improve performance and efficiency. |
## 💡 Credits
- Thanks to [daffyyyy](https://github.com/daffyyyy) for the respawn function from their plugin [CS2-SimpleAdmin](https://github.com/daffyyyy/CS2-SimpleAdmin), which is used in this project.
- Thanks to [zakriamansoor47](https://github.com/zakriamansoor47) for the beacon function from their plugin [SLAYER_Duel](https://github.com/zakriamansoor47/SLAYER_Duel), which is used in this project.

## 📄 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
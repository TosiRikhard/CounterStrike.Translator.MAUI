
# CounterStrike Translator

CounterStrike Translator is a chat translator for CounterStrike. It supports different message formats and can translate text to a specific language or the last used language.

## Features

* Translate incoming messages from players.
* Translate commands from own user.
* Customizable settings for translations.
* Supports specific commands and parameters for fine-grained control over translations.

## Commands and Parameters

In CounterStrike Translator, you can use several commands and parameters to customize the translation process:

### `!tr`

The `!tr` command allows you to translate a message to a specific language. The format of the command is `!tr language_code message`, where `language_code` is the language you want to translate to, and `message` is the text you want to translate.

For example, `!tr es Hello` will translate "Hello" into Spanish.

### `!trl`

The `!trl` command allows you to translate a message to the last used language. The format of the command is `!trl message`, where `message` is the text you want to translate.

For example, if the last used language was Spanish, `!trl Hello` will translate "Hello" into Spanish.

### `language_code`

The `language_code` parameter specifies the language you want to translate to. It should be a valid ISO 639-1 language code, such as `en` for English or `es` for Spanish. If the language code is not valid or recognized, the command will not execute.

### `!allchat` and `!teamchat`

The `!allchat` and `!teamchat` parameters allow you to specify where the translated message will be sent.

If you include `!allchat` in your command, the translated message will be sent to all chat. For example, `!tr es !allchat Hello` will translate "Hello" into Spanish and send it to all chat.

If you include `!teamchat` in your command, the translated message will be sent to your team's chat. For example, `!tr es !teamchat Hello` will translate "Hello" into Spanish and send it to your team's chat.

If neither `!allchat` nor `!teamchat` is specified, the translated message will be sent to the same chat type as the source message.

## Usage

The application provides a user-friendly interface for viewing and managing translations. The main screen displays a list of messages along with their senders, source languages, and target languages. 

Users can also set their preferences in the settings panel:

* **Don't Translate messages from profile names above**: Users can specify profiles whose messages should not be translated.
* **Only translate all chat**: If checked, only messages from all chat will be translated.
* **Send translations to team chat**: If checked, translations will be sent to the team chat.
* **Default translating language**: Users can set a default language for translations.
* **Don't translate Languages (separate by comma)**: Users can specify languages that should not be translated.

To save changes made to the settings, click on the "Save Changes" button.

## Getting Started

This project is built with the MAUI framework. Make sure you have the necessary environment set up to run MAUI projects. Clone the repository and open the project in your preferred IDE.

```bash
git clone <repository url>
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License

[MIT](https://choosealicense.com/licenses/mit/)

This is a basic README file and does not contain all the information about the project. You may want to add more sections such as "Installation", "API Reference", "Tests", "Deployment" etc., depending on the complexity of your project. Also, you might want to elaborate more on certain sections, add code examples, screenshots, etc.

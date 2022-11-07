window.onload = async () => {
	const footer = document.getElementById('updates');

	try {
		// Remote
		const res = await axios.get('https://raw.githubusercontent.com/sefinek24/genshin-impact-reshade-2023/main/Data/www/api/remote.json');

		// Instruction
		if (local.version === res.data.version) {
			footer.innerHTML = `
                <div class="updates-header">
                    ${Math.floor(Math.random() * 2) ? '<img src="Data/www/images/thumbsup.gif" alt="✅">' : '<img src="Data/www/images/success.gif" alt="✅">'}<br>
                    ✅ Your downloaded release is up-to-date!
                </div>

                <p>
                    🌍 Version: <code style="color:#00ff00">v${local.version}</code> from <code>${local.date}</code><br>
                    ⏰ Last update: <code>${local.lastUpdate}</code>
                </p>`;
		} else {
			footer.innerHTML = `
                <div class="updates-header">
                    ${Math.floor(Math.random() * 2) ? '<img src="Data/www/images/blobcat.gif" alt="✅">' : '<img src="Data/www/images/dancing-anime-girl.gif" alt="✅">'}<br>
                    📥 New version is available!
                </div>

                <code style="color:#00fff7">v${local.version}</code> → <code style="color:#00ff00">v${res.data.version}</code>
                <p>🤔 Run the file <u>RUN GENSHIN IMPACT.cmd</u> to synchronize the local repository with the remote one.</p>
                <p>
                    📂 Your version: <code>v${local.version}</code> from <code>${local.date}</code><br>
                    ⏰ Last update: <code>${local.lastUpdate}</code>
                </p>`;
		}
	} catch (err) {
		console.log('=========================== DEBUG ===========================');
		console.error(err);
		console.log(window.navigator.userAgent);
		console.log('=========================== DEBUG ===========================');

		switch (true) {
		case err.response ? err.response.status === 404 : false: {
			return footer.innerHTML = `
                <div class="updates-header">
                    <img src="Data/www/images/error.gif" alt="❌"><br>
                    ❌ ${err.message}
                </div>
                A required file was not found. Report this error on GitHub.

                <p><a href="https://github.com/sefinek24/genshin-impact-reshade-2023/issues" target="_blank">Issues · sefinek24/genshin-impact-reshade-2023</a></p>
            `;
		}

		case err.code === 'ERR_NETWORK': {
			return footer.innerHTML = `
                <div class="updates-header">
                    <img src="Data/www/images/error.gif" alt="❌"><br>
                    🌍 ${err.message}
                </div>
                Something went wrong. Are you connected to the internet?
            `;
		}

		default: {
			footer.innerHTML = `
                <div class="updates-header">
                    <img src="Data/www/images/error.gif" alt="❌"><br>
                    ❌ ${err.message}
                </div>
                Something went wrong. Re-clone the files from the repository using the <b>git clone</b> command.

                <p><code>git clone https://github.com/sefinek24/genshin-impact-reshade-2023.git</code></p>
            `;
		}
		}
	}
};
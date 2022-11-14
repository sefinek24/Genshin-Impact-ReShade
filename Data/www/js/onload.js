function debug(err) {
	console.log('=========================== DEBUG ===========================');
	console.log(window.navigator.userAgent);
	console.error(err);
	console.log('=========================== DEBUG ===========================');
}


window.onload = async () => {
	const footer = document.getElementById('updates');

	try {
		const res = await axios.get('https://raw.githubusercontent.com/sefinek24/genshin-impact-reshade-2023/main/Data/www/api/remote.json');
		console.log(local.version);
		console.log(res.data.version);

		const msg = `
			<code style="color:#00fff7">${local.build ? local.build : 'Unknown'}</code> → <code style="color:#00ff00">${res.data.build ? res.data.build : 'Unknown'}</code>
            <p>🤔 Run the file <u>UPDATE.cmd</u> to synchronize the local repository with the remote one.</p>
            <p>
            	📂 Your version: <code><span title="Build ${local.build}">v${local.version}</span></code> from <code>${local.releaseVersion}</code><br>
                ⏰ Last update: <code>${local.lastUpdate}</code>
            </p>
		`;

		switch (true) {
		// new
		case !res.data.build: case !res.data.version: {
			footer.innerHTML = `
                <div class="updates-header">
                    <img src="Data/www/images/error.gif" alt="❌"><br>
                    😿 Your version is not supported.
                </div>

                <code style="color:#f04947">${local.build ? local.build : 'Unknown'}</code></code>
                <p>
                	Download the module again from the GitHub repository using the git command in Terminal.<br>
                	<code>git clone https://github.com/sefinek24/genshin-impact-reshade-2023.git</code>
                </p>
                <p>
                    📂 Your version: <code>v${local.version}</code> from <code>${local.releaseVersion}</code><br>
                    ⏰ Last update: <code>${local.lastUpdate}</code>
                </p>`;

			debug(res.data);

			break;
		}

		// 2
		case local.build !== res.data.build: {
			footer.innerHTML = `
                <div class="updates-header">
                    ${Math.floor(Math.random() * 2) ? '<img src="Data/www/images/blobcat.gif" alt="✅">' : '<img src="Data/www/images/dancing-anime-girl.gif" alt="✅">'}<br>
                    📥 New build is available!
                </div>

                ${msg}`;
			break;
		}

		// 3
		case local.version !== res.data.version: {
			footer.innerHTML = `
                <div class="updates-header">
                    ${Math.floor(Math.random() * 2) ? '<img src="Data/www/images/blobcat.gif" alt="✅">' : '<img src="Data/www/images/dancing-anime-girl.gif" alt="✅">'}<br>
                    📥 New version is available!
                </div>

                ${msg}`;
			break;
		}

		// 4
		default: {
			footer.innerHTML = `
                <div class="updates-header">
                    ${Math.floor(Math.random() * 2) ? '<img src="Data/www/images/thumbsup.gif" alt="✅">' : '<img src="Data/www/images/success.gif" alt="✅">'}<br>
                    ✅ Your downloaded release is up-to-date!
                </div>

                <p>
                    🌍 Version: <code style="color:#00ff00"><span title="Build ${local.build}">v${local.version}</span> </code> from <code>${local.releaseVersion}</code><br>
                    ⏰ Last update: <code>${local.lastUpdate}</code>
                </p>`;
		}
		}
	} catch (err) {
		debug(err);

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
function debug(err) {
	console.log('=========================== DEBUG ===========================');
	console.log(window.navigator.userAgent);
	console.error(err);
	console.log('=========================== DEBUG ===========================');
}


window.onload = async () => {
	const footer = document.getElementById('updates');

	try {
		const res = await axios.get('https://raw.githubusercontent.com/sefinek24/Genshin-Impact-ReShade/main/Data/Web/api/remote.json');

		const msg = () => `
			<code style="color:#00fff7">${local.version ? `v${local.version}` : 'Unknown'}</code> → <code style="color:#00ff00">${res.data.version ? `v${res.data.version}` : 'Unknown'}</code>
            <p>🤔 Go to the application Genshin Impact Mod Pack and click Check for updates.</p>
            <p>
            	📂 Your version: <code>v${local.version}</code> from <code>${local.releaseVersion}</code><br>
                ⏰ Last update: <code>${res.data.lastUpdate}</code>
            </p>
		`;

		switch (true) {
		case !res.data.version: {
			footer.innerHTML = `
                <div class="updates-header">
                    <img src="Data/Web/images/error.gif" alt=""><br>
                    😿 Your version is deprecated
                </div>

                <code style="color:#f04947">${local.version ? `v${local.version}` : 'Unknown'}</code></code>
                <p>
                	Download this software again using our installer.<br>
                	<a href="https://sefinek.net/genshin-impact-reshade" style="color:dodgerblue">sefinek.net/genshin-impact-reshade</a>
                </p>
                <p>
                    📂 Your version: <code>v${local.version}</code> from <code>${local.releaseVersion}</code><br>
                    ⏰ Last update: <code>${res.data.lastUpdate}</code>
                </p>`;

			debug(res.data);

			break;
		}

		case local.version !== res.data.version: {
			footer.innerHTML = `
                <div class="updates-header">
                    ${Math.floor(Math.random() * 2) ? '<img src="Data/Web/images/blobcat.gif" alt="">' : '<img src="Data/Web/images/dancing-anime-girl.gif" alt="">'}<br>
                    📥 New version is available
                </div>

                ${msg()}`;
			break;
		}

		default: {
			footer.innerHTML = `
                <div class="updates-header">
                    ${Math.floor(Math.random() * 2) ? '<img src="Data/Web/images/thumbsup.gif" alt="">' : '<img src="Data/Web/images/success.gif" alt="">'}<br>
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
                    <img src="Data/Web/images/error.gif" alt=""><br>
                    ❌ ${err.message}
                </div>
                A required file was not found. Report this error on GitHub.

                <p><a href="https://github.com/sefinek24/Genshin-Impact-ReShade/issues" target="_blank">Issues · sefinek24/Genshin-Impact-ReShade</a></p>
            `;
		}

		case err.code === 'ERR_NETWORK': {
			return footer.innerHTML = `
                <div class="updates-header">
                    <img src="Data/Web/images/error.gif" alt=""><br>
                    🌍 ${err.message}
                </div>
                Something went wrong. Are you connected to the internet?
            `;
		}

		default: {
			footer.innerHTML = `
                <div class="updates-header">
                    <img src="Data/Web/images/error.gif" alt=""><br>
                    ❌ ${err.message}
                </div>
                <p>
                	Something went wrong. Download module again using our installer.<br>
                	<a href="https://sefinek.net/genshin-impact-reshade" style="color:dodgerblue">sefinek.net/genshin-impact-reshade</a>
                </p>
            `;
		}
		}
	}
};
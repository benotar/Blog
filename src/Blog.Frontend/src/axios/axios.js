import axios from "axios";
import {useAppStore} from "../zustand/useAppStore.js";

const $axios = axios.create({
    baseURL: "/api",
    timeout: 50000,
    withCredentials: true,
    headers: {
        "Content-Type": "application/json"
    }
});

$axios.interceptors.request.use(config => {

    const storeState = useAppStore.getState();

    if (storeState.tokens?.accessToken) {
        const accessToken = storeState.tokens.accessToken;
        config.headers.Authorization = `Bearer ${accessToken}`;
    }

    return config;
}, error => {
    return Promise.reject(error);
});

$axios.interceptors.response.use(response => response,
    async error => {

        const originalRequest = error.config;
        const errorResponse = error.response;

        if (errorResponse.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            let storeState = useAppStore.getState();

            if (!storeState.currentUser && !storeState.tokens?.refreshToken) {
                const refreshFailure = storeState.refreshFailure;
                refreshFailure("You have been logged out due to inactivity. Please log in again.");
                return Promise.reject();
            }

            const refreshStart = storeState.refreshStart;
            const refreshSuccess = storeState.refreshSuccess;
            const refreshToken = storeState.tokens.refreshToken;
            const currentUser = storeState.currentUser;

            try {
                refreshStart();
                const {data} = await $axios.post("token/refresh", {
                    userId: currentUser.id,
                    refreshToken
                });

                if (data.isSucceed === false) {
                    console.log(data.errorCode);
                    const refreshFailure = storeState.refreshFailure;
                    refreshFailure("You have been logged out due to inactivity. Please log in again.");
                    return Promise.reject();
                }

                refreshSuccess(data.payload);
                storeState = useAppStore.getState();

                if (!storeState.tokens?.accessToken) {
                    const refreshFailure = storeState.refreshFailure;
                    refreshFailure("You have been logged out due to inactivity. Please log in again.");
                    return Promise.reject();
                }
                const accessToken = storeState.tokens.accessToken;
                originalRequest.headers.Authorization = `Bearer ${accessToken}`;

                return $axios(originalRequest);
            } catch (catchError) {
                const refreshFailure = storeState.refreshFailure;
                refreshFailure();
                return Promise.reject(catchError);
            }
        }
        return Promise.reject(error);
    });

export default $axios;
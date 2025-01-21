import {create} from "zustand";
import {devtools, persist} from "zustand/middleware";
import {createUserSlice} from "./user/userSlice.js";
import {createThemeSlice} from "./theme/themeSlice.js";

export const useAppStore = create(
    devtools(
        persist(
            (...a) => ({
                ...createUserSlice(...a),
                ...createThemeSlice(...a)
            }),
            {name: "app-store", version: 1}
        ),
        {name: "AppStoreDevTools"}
    )
);
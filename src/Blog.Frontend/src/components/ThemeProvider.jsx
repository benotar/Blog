import {useAppStore} from "../zustand/useAppStore.js";

export default function ThemeProvider({children}) {

    const theme = useAppStore((state) => state.theme);

    return (
        <div className={theme}>
            <div className="bg-white text-gray-700 dark:text-gray-200 dark:bg-[rgb(16,23,42)] min-h-screen">
                {children}
            </div>
        </div>
    );
}
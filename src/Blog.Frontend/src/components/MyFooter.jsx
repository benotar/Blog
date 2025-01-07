import {Footer} from "flowbite-react";
import {Link} from "react-router-dom";

export default function MyFooter() {
    return (
        <Footer container className="border border-t-8 border-teal-500">
            <div className="w-full max-w-7xl mx-auto">
                <div className="grid w-full justify-between sm:flex md:grid-cols-1">
                    <div className="mt-5">
                        <Link to="/" className="self-center whitespace-nowrap text-lg
                        sm:text-xl font-semibold dark:text-white">
                            <span
                                className="px-2 py-1 bg-gradient-to-r from-indigo-500
                                via-purple-500 to-pink-500 rounded-lg text-white"
                            >
                                Benotar_&apos;s
                            </span>
                            Blog
                        </Link>
                    </div>
                    <div className="grid grid-cols-2 gap-8 mt-4 sm:grid-cols-3 sm:gap-6">
                        <div>
                            <Footer.Title title="About"/>
                            <Footer.LinkGroup col>
                                <Footer.Link
                                    href="/about"
                                    target="_blank"
                                    rel="noopener noreferrer"
                                >
                                    Benotar_&apos;s Blog
                                </Footer.Link>
                            </Footer.LinkGroup>
                        </div>
                        <div>
                            <Footer.Title title="Follow Us"/>
                            <Footer.LinkGroup col>
                                <Footer.Link

                                    href="https://github.com/benotar"
                                    target="_blank"
                                    rel="noopener noreferrer"
                                >
                                    GitHub
                                </Footer.Link>
                                <Footer.Link
                                    href="https://www.linkedin.com/in/benotar"
                                    target="_blank"
                                    rel="noopener noreferrer"
                                >
                                    LinkedIn
                                </Footer.Link>
                                <Footer.Link
                                    href="https://t.me/benotaar"
                                    target="_blank"
                                    rel="noopener noreferrer"
                                >
                                    Telegram
                                </Footer.Link>
                                <Footer.Link
                                    href="https://www.instagram.com/benotar_?igsh=eWphdHZncXZ5ZThk&utm_source=qr"
                                    target="_blank"
                                    rel="noopener noreferrer"
                                >
                                    Instagram
                                </Footer.Link>
                            </Footer.LinkGroup>
                        </div>
                        <div>
                            <Footer.Title title="Legal"/>
                            <Footer.LinkGroup col>
                                <Footer.Link href="#">
                                    Privacy Policy
                                </Footer.Link>
                                <Footer.Link href="#">
                                    Terms &amp; Conditions
                                </Footer.Link>
                            </Footer.LinkGroup>
                        </div>
                    </div>
                </div>
                <Footer.Divider/>
                <div className="w-full sm:flex sm:items-center sm:justify-between">
                    <Footer.Copyright href="#" by="Benotar_&apos;s blog" year={new Date().getFullYear()}/>
                </div>
            </div>
        </Footer>
    );
}
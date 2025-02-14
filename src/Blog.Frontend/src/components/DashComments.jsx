import {Table} from "flowbite-react";
import DashItem from "./DashItem.jsx";
import {useState} from "react";

const DashComments = () => {
    const [items, setItems] = useState([]);
    const [showMore, setShowMore] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [commentIdToDelete, setCommentIdToDelete] = useState(null);
    const [itemsPerPage] = useState(9);


    return (
        <DashItem
            fetchUrl={`comment/get-comments`}
            setItems={setItems}
            setShowMore={setShowMore}
            itemsLength={items.length}
            showMoreUrl={`post/get-posts?startIndex=${items.length}/limit=${itemsPerPage}`}
            setShowModal={setShowModal}
            urlToDelete={`post/delete-post/${commentIdToDelete}`}
            itemIdToDelete={commentIdToDelete}
            showModal={showModal}
            showMore={showMore}
        >
            <Table.Head>
                <Table.HeadCell>Date updated</Table.HeadCell>
                <Table.HeadCell>Comment content</Table.HeadCell>
                <Table.HeadCell>Count of likes</Table.HeadCell>
                <Table.HeadCell>Post id</Table.HeadCell>
                <Table.HeadCell>User id</Table.HeadCell>
                <Table.HeadCell><span>Edit</span></Table.HeadCell>
            </Table.Head>
            {
                items.map((item) =>
                    <Table.Body
                        className="divide-y"
                        key={item.id}
                    >
                        <Table.Row
                            className="bg-white dark:border-gray-700 dark:bg-gray-800"
                        >
                            <Table.Cell>
                                {new Date(item.updatedAt).toLocaleDateString()}
                            </Table.Cell>
                            <Table.Cell>
                                {item.content}
                            </Table.Cell>
                            <Table.Cell>
                                {item.countOfLikes}
                            </Table.Cell>
                            <Table.Cell>
                                {item.postId}
                            </Table.Cell>
                            <Table.Cell>
                                {item.author.id}
                            </Table.Cell>
                            <Table.Cell>
                                    <span
                                        onClick={() => {
                                            setShowModal(true);
                                            setCommentIdToDelete(item.id);
                                        }}
                                        className="font-medium text-red-500 hover:underline cursor-pointer"
                                    >
                                        Delete
                                    </span>
                            </Table.Cell>
                        </Table.Row>
                    </Table.Body>
                )
            }
        </DashItem>
    );
};


export default DashComments;